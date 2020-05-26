const gulp = require("gulp");
const uglify = require('gulp-uglify');
const concat = require('gulp-concat');
const del = require('del');
const postcss = require("gulp-postcss");
const cssnano = require("cssnano");
const imagemin = require('gulp-imagemin');

function clean() {
    return del([
        './js/**/*',
        './css/**/*',
        './images/**/*',
    ]);
}

function vendorjs() {
    return gulp.src([
            'src/js/vendor/jquery-3.3.1.slim.min.js',
            'src/js/vendor/bootstrap.bundle.min.js',
            'src/js/vendor/angular.min.js',
            'src/js/vendor/moment.min.js',
            'src/js/vendor/angular-moment.js',
            'src/js/vendor/ui-bootstrap-tpls.js',
            'src/js/vendor/angular-modal-service.js',
            'src/js/vendor/sharer.min.js',
        ])
        .pipe(concat('vendor.js'))
        .pipe(gulp.dest('./js'));
}

function customjs() {
    return gulp.src([
            'src/js/*.js'
        ])
        .pipe(concat('site.js'))
        .pipe(uglify())
        .pipe(gulp.dest('./js'));
}

function fonts() {
    return gulp
        .src('src/css/fonts/*.*')
        .pipe(gulp.dest('./css/fonts'));
}

function vendorcss() {
    return gulp
        .src([
            'src/css/bootstrap.min.css',
            'src/css/Animate.css',
        ])
        .pipe(concat('vendor.css'))
        .pipe(postcss([cssnano()]))
        .pipe(gulp.dest('./css'));
}

function customcss() {
    return gulp
        .src([
            'src/css/nav-bar.css',
            'src/css/MobileMenu.css',
            'src/css/custom.css',
            'src/css/homepage.css',
        ])
        .pipe(concat('site.css'))
        .pipe(postcss([cssnano()]))
        .pipe(gulp.dest('./css'));
}

function images() {
    return gulp.src('src/images/*.*')
        .pipe(imagemin())
        .pipe(gulp.dest('./images'));
}

const css = gulp.parallel(fonts, customcss, vendorcss);
const javascript = gulp.parallel(vendorjs, customjs);

exports.default = gulp.series(clean, javascript, css, images);
