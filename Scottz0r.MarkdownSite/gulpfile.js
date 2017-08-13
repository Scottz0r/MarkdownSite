// Include gulp
var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var rename = require('gulp-rename');

gulp.task('lib', function(){
    return gulp.src('UI/lib/*.js')
        .pipe(concat('lib.js'))
        .pipe(rename('lib.min.js'))
        .pipe(uglify({ mangle: true }))
        .pipe(gulp.dest('wwwroot'));
});

gulp.task('static', function(){
    return gulp.src(['UI/index.html', 'UI/site.css', 'UI/github-markdown.css'])
        .pipe(gulp.dest('wwwroot'));
});

gulp.task('src', function(){
    return gulp.src('UI/md-display.js')
        .pipe(uglify())
        .pipe(rename('site.min.js'))
        .pipe(gulp.dest('wwwroot'));
});

gulp.task('default', ['lib', 'static', 'src']);
