


class ClientAPI {

    constructor(url) {
        this.url = url;
        this.authInfo = null;
    }


    //Establece los valores de udentidad para las llamadas a la API a partit de
    //los valores gestionados por la biblioteca de OIDC
    async _setAuthInfo() {
        this.authInfo = await getAuthInfo();
        return (this.authInfo != null);
    }


    //Obtiene la confiración de encabezados para las llamdas de API
    _getAxiosConfig(method, querystring) {

        if (this.authInfo == null) return null;
        var url = this.url;

        if (querystring != undefined) {
            url = url + querystring;
        }

        // url = url + "?_=" + new Date().getMilliseconds();

        return {
            method: method,
            url: url,
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Content-Type': 'application/json',
                'uid': this.authInfo.uid,
                'Authorization': `Bearer ${this.authInfo.jwt}`
            }
        };

    }

    async checkAuthInfo() {
        if (this.authInfo == null) {
            let authAvailable = await this._setAuthInfo();
            return authAvailable;
        }
        return true;
    }

    existeAuthInfo() {
        return (this.authInfo != null);
    }


    _obtieneDatosDesdeRespuesta(httpresponse) {

        try {
            let respuesta = new respuestaAPI(parseInt(httpresponse.status), null, false, false);

            switch (respuesta.estado) {
                case 200:
                    //DAtos desde AXIOS
                    respuesta.datos = httpresponse.data;
                    break;

                default:
                    respuesta.es_error = true;
                    break;
            }

            return respuesta;
        } catch (e) {
            return new respuestaAPI(0, e, true, false);
        }
    }

    async getMetadata() {

        let authAvailable = await this.checkAuthInfo();

        if (authAvailable) {
            const config = this._getAxiosConfig("get", "metadata");
            let res = await axios(config);
            return this._obtieneDatosDesdeRespuesta(res);
        }
        else {
            return null;
        }

    }



}



//Representa una respuesta estándar a la API de PIKA
class respuestaAPI {

    constructor(estado, datos, es_error, reintentar) {
        this.datos = datos;
        this.estado = estado;
        this.es_error = es_error;
        this.reintentar = reintentar;
    }

}