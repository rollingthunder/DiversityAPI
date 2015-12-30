cd $PSScriptRoot

Remove-Item -ErrorAction SilentlyContinue -Path authority.key, authority.crt, authority.pfx

openssl req -x509 -sha256 -days 365 -nodes -config ./authority.cnf -newkey rsa:2048 -keyout authority.key -out authority.crt
openssl pkcs12 -export -nodes -in authority.crt -inkey authority.key -out authority.pfx -passout pass:
