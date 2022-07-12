systemctl stop pika.identity.service;
systemctl stop pika.api.service;
cp /home/arguz/pika/identity/wwwroot/css/site.css .
cp /home/arguz/pika/identity/wwwroot/logo_pika.png logo_pika_identity.png;
cp /usr/share/nginx/html/config.json .;
cp /usr/share/nginx/html/favicon.ico .;
cp /usr/share/nginx/html/assets/images/logo_pika.png .
cp /usr/share/nginx/html/assets/images/sidebar_pika.png .
cp /home/arguz/pika/api/appsettings.json ./appsettings-api.json;
cp /home/arguz/pika/identity/appsettings.json ./appsettings-identity.json;
rm /home/arguz/pika/api/*.*;
rm -R -- /home/arguz/pika/api/*/;
rm /home/arguz/pika/identity/*.*;
rm -R -- /home/arguz/pika/identity/*/;
rm /usr/share/nginx/html/*.*;
rm -R -- /usr/share/nginx/html/*/;
cp -a ./api/. /home/arguz/pika/api;
cp -a ./identity/. /home/arguz/pika/identity;
cp -a ./html/. /usr/share/nginx/html/;
cp config.json /usr/share/nginx/html/;
cp appsettings-identity.json /home/arguz/pika/identity/appsettings.json;
cp appsettings-api.json /home/arguz/pika/api/appsettings.json;
cp -rf logo_pika.png /usr/share/nginx/html/assets/images/logo_pika.png
cp -rf sidebar_pika.png /usr/share/nginx/html/assets/images/sidebar_pika.png
cp -rf favicon.ico  /usr/share/nginx/html/favicon.ico
cp -rf favicon.ico /home/arguz/pika/identity/wwwroot/favicon.ico
cp -rf logo_pika_identity.png /home/arguz/pika/identity/wwwroot/logo_pika.png;
cp -rf site.css /home/arguz/pika/identity/wwwroot/css/site.css
systemctl start pika.identity.service;
nginx -s reload;
systemctl start pika.api.service;

