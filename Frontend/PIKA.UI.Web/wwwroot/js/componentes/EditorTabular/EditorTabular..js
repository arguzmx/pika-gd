//El editor tabular se encarga de realizar las operaciones
//de CRUD para el tipo de entidad exietente en el endpoint de creación
class editorTabular {

    constructor(url) {
        this.url = url
        this.client = new ClientAPI(url);
    }

    //Punto de entrada para el editor
    inicializarEditorTabular() {
        this.client.getMetadata().then((respuesta) => {
            if (!respuesta.es_error) {
                console.log(respuesta.datos);
                //Aqui va el códgio para la creación de los objetos de Datatables

            } else {

            }
        });

    }

}



//señal de iniciaclizació desde JQUERY
$(function () {
    let editor = new editorTabular("http://localhost:5000/api/v1.0/org/Dominio/");
    editor.inicializarEditorTabular();
});
