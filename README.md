# OTITO.io

Why not webpack?

Site isn't built using modular js, it's just a bunch of vendor files  
Gulp improves things until such a time that client side can be refactored into a modern js app


## Client side build

Install gulp

```bash
$ npm install --global gulp-cli
```

Run build process

```bash
$ cd src/OTITO.Web/wwwroot/
$ npm ci
$ gulp
```

## Publish website 

```
# run client side build first
$ cd {project root}
$ dotnet publish OTITO.sln -o dist
```

## Infrastructure

```bash
# website is deployed to 
/var/www/otito

# nginx config @
/etc/nginx/sites-available/default

# service config
/etc/systemd/system/otito.service
```

systemd cheatsheet

```bash
$ systemctl status otito
$ systemctl restart otito
```