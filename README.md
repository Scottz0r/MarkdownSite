# Markdown Site

This is a webserver, built on ASP.Net core, that displays pages created from a directory of Markdown files. The UI is designed as a SPA that can navigate between Markdown pages. Pages and navigation is built off of files within the source directory, so configuration is simple.

## Requirements

This requires the .Net core runtime if self hosting in Kestrel.

### Build Requirements

Nodejs and NPM are required to build the UI components.

## Configuration

The server is configured through environment variables:

* `MDSSourceDirectory` - The source directory containing the Markdown files. Note: Nested folders are not yet supported.
* `MDSFileExtension` (optional) - The extension used for Markdown files. Default is .md.

ASP.Net server can be configured via environment variables:

* `ASPNETCORE_ENVIRONMENT` - Set to 'Development' to see Debug logging. 
* `ASPNETCORE_URLS` - Url to run server (ex: ASPNETCORE_URLS=http://localhost:4100).

## Build / Publish

The UI of this project requires node and npm. Run 'build' and 'gulp' tasks from NPM to copy items to wwwroot. When publishing the wwwroot will need to be copied manually to the distribution folder (fixing this in the fiture).

## Projects Used

The following were used in the development of this project:

* [Page.js](https://github.com/visionmedia/page.js) - Client side routing.
* [Showdown](https://github.com/showdownjs/showdown) - Client side Markdown renderer.
* [github-markdown-css](https://github.com/sindresorhus/github-markdown-css) - CSS stylesheet for Markdown.