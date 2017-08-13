declare function page(route: string, callback: (...args: any[]) => any);
declare function page(name: string);
declare function page();

// Results from file listing.
interface ListResult {
    files: string[]
}

// Results from markdown file request.
interface FileResult {
    content: string;
    lastModifiedUtc: string;
}

// Error result object.
interface ErrorResult {
    statusCode: number;
    message: string;
}

// Dto wrapper for results.
// Note: error will take precedence over data if both present.
interface DtoWrapper<T> {
    id: string;
    data?: T;
    error?: ErrorResult;
}

// This class manages a local cache of pages that have already been 
// downloaded by the client.
class PageCache {
    private readonly cacheObj: object = {};

    isPageCached = (key: string): boolean => {
        return this.cacheObj.hasOwnProperty(key);
    }

    putCache = (key: string, htmlContent: string): void => {
        this.cacheObj[key] = htmlContent;
    }

    // Read item from cache. Returns an error message if item 
    // not in cache.
    readCache = (key: string): string => {
        if (this.isPageCached(key)) {
            return this.cacheObj[key];
        } else {
            return "Error: Page is not cached.";
        }
    }
}

// This class is responsible for loading markdown documents
// that are returned by the API.
class PageDisplayer {
    private readonly ERR: string = 'Error while fetching data.';
    private readonly converter: showdown.Converter = new showdown.Converter({ ghCodeBlocks: true });
    private readonly pageCache: PageCache = new PageCache();
    private readonly $content: JQuery = $('#content');

    // Load the given markdown document (by name).
    loadPage = (name: string): void => {
        this.$content.hide();
        // Check local cache before making request.
        if (this.pageCache.isPageCached(name)) {
            this.setContent(this.pageCache.readCache(name));
        } else { // Make API call.
            this.$content.hide();
            $.getJSON('/file/' + name)
            .done(res => this.processResult(name, res))
            .fail(this.error);
        }
    }

    // Process a successful result, converting Markdown into HTML.
    private processResult = (name: string, result: DtoWrapper<FileResult>): void => {
        let resultContent: string;
        try {
            // If somehow an error got through with OK status
            if (result.error != null) {
                resultContent = this.ERR;
            } else {
                resultContent = this.converter.makeHtml(result.data.content);
                this.pageCache.putCache(name, resultContent);
            }
        } catch (e) {
            resultContent = this.ERR;
        }
        this.setContent(resultContent);
    }

    // Handle error from API.
    private error = (xhr: JQueryXHR): void => {
        let errorContent: string;
        try {
            let resJSON: DtoWrapper<ErrorResult> = xhr.responseJSON;
            errorContent = resJSON.error.message;
        } catch (e) {
            errorContent = this.ERR;
        }
        this.setContent(errorContent);
    }

    // Set the document and do a little animation to simulate an
    // angular like route animation.
    private setContent = (content: string): void => {
        this.$content.html(content);
        this.$content.fadeIn();
    }
}

// This class is responsible for loading a navigation bar of documents
// that the api can serve.
class PageLister {
    private readonly ERR = "Error fetching pages.";
    private readonly $pages: JQuery = $('#pages');

    // Initialize the list of files in the navigation bar.
    initPages = (): void => {
        $.getJSON('/file/__list')
        .done(this.processPagesResult)
        .fail(this.error);
    }

    // Process a successful file list request.
    private processPagesResult = (result: DtoWrapper<ListResult>): void => {
        try {
            // If an error somehow got through
            if (result.error != null) {
                this.setNavContent(result.error.message);
            } else {
                let navHtml = this.createNavLinks(result.data.files);
                this.setNavContent(navHtml);
            }
        } catch (e) {
            this.setNavContent(this.ERR);
        }
    }

    // Build a li of links to documents. The index will be injected
    // by default.
    private createNavLinks = (files: string[]): string =>  {
        let navHtml: string = '<ul>';

        navHtml += `<li><a href='./'>Home</a></li>`;

        $.each(files, (idx: number, val: string) => {
            // Everything but home link
            if(val !== 'index') {
                navHtml += `<li><a href='./${val}'>${val}</a></li>`;
            }
        });

        return navHtml;
    }

    // Display error message on JQuery fail.
    private error = (xhr: JQueryXHR): void => {
        try {
            let resJSON: DtoWrapper<ErrorResult> = xhr.responseJSON;
            this.setNavContent(resJSON.error.message);
        } catch (e) {
            this.setNavContent(this.ERR);
        }
    }

    // Animate the nav loading in.
    private setNavContent = (navContent: string): void => {
        this.$pages.hide();
        this.$pages.html(navContent);
        this.$pages.fadeIn();
    }
}

// Start page router and get Nav links when DOM loaded.
$(document).ready(() => {
    let pageDisplayer = new PageDisplayer();
    let pageLister = new PageLister();

    pageLister.initPages();

    // Router setup.
    page('/', () => pageDisplayer.loadPage('index'));
    page('/:name', (ctx) => pageDisplayer.loadPage(ctx.params.name));
    page();
});
