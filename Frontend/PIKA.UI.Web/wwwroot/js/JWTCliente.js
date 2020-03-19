var JWTCliente = (function () {


    var tokenValido = function () {
        var token = this.token();
        if (token == undefined || token == null || token == '') {
            return false;
        }
        var fechatoken = new Date(parseInt(Cookies.get('jwt_to')));
        var fechaactual = new Date();
        return (fechatoken.getMilliseconds() > fechaactual.getMilliseconds());
    };

    var token = function () {
        return Cookies.get("jwt");
    };


    var nuevoToken = (){

    }


    return {
        token: token,
        tokenValido: tokenValido,
        nuevoToken: nuevoToken
    }


})();