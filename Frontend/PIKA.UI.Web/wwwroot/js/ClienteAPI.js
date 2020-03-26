class ClientAPI {

    constructor(url) {
        this.url = url;
        this.authInfo = null;
        
        var me = this;
        getAuthInfo().then(
            function (data) {
                me.authInfo = data;
                me.initControl();
            }
        );
    }

    _getAxiosConfig(method, querystring) {

        if (this.authInfo == null) return null;
        var url = this.url;

        if (querystring != undefined) {
            url = url  + querystring;
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

    //Inicializa la instancia del editor tabular
    initControl() {
        this.getMetadata();
    }


    async getMetadata() {
        const config = this._getAxiosConfig("get","metadata");

        console.log(config);
        let res = await axios(config);

        console.log(res);

    }



}