declare function page(route: string, callback: (...args: any[]) => any);
declare function page(name: string);
declare function page();

interface ListResult {
    files: string[]
}

class PageDisplayer {
    private converter: showdown.Converter = new showdown.Converter();
    private $content: JQuery = $('#content');

    loadContent = (name: string) => {
        this.$content.hide();
        $.ajax('/file/' + name)
        .done(this.setContent)
        .fail(this.error);
    }

    setContent = (result: string) => {
        let html = this.converter.makeHtml(result);
        this.$content.html(html);
        this.$content.fadeIn();
    }

    error = (xhr: JQueryXHR) => {
        // TODO
        this.$content.text('Error');
        this.$content.fadeIn();
    }
}

class PageLister {

    private $pages: JQuery = $('#pages');

    initPages = () => {
        $.getJSON('/file/__list')
        .done((res: ListResult) => {
            // Force home link first
            this.$pages.html('<ul>');
            this.$pages.append(`<li><a href='./'>Home</a></li>`);
            $.each(res.files, (idx, val) => {
                // Everything but home link
                if(val !== 'index') {
                    let navLink = `<li><a href='./${val}'>${val}</a></li>`;
                    this.$pages.append(navLink)
                }
            });
            this.$pages.append('</ul>');
        });
    }
}

$(document).ready(() => {
    let pageDisplayer = new PageDisplayer();
    let pageLister = new PageLister();

    pageLister.initPages();

    page('/', () => pageDisplayer.loadContent('index'));
    page('/:name', (ctx) => pageDisplayer.loadContent(ctx.params.name));
    page();
});
