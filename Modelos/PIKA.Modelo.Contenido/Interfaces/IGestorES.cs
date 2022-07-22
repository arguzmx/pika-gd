using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Modelo.Contenido
{
    public interface IGestorES
    {
        /// <summary>
        /// Determina si la conexión es válida en base a los parámetros de configuración
        /// </summary>
        /// <returns></returns>
        bool ConexionValida();


        Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido, FileInfo informacion, bool sobreescribir);
        Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, string archivoFuente, FileInfo informacion, bool sobreescribir);

        Task<byte[]> LeeBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension );

        Task EliminaBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension);

        Task<byte[]> LeeThumbnailBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension);
        Task<long> EscribeThumbnailBytes(string ParteId, string ElementoId, string VersionId,  byte[] contenido);
        Task<long> EscribeOCRBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido);
        Task<byte[]> LeeOCRBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension);

        Task<bool> Elimina(string RutaArchivo);

        /// <summary>
        /// Determina si el gestort tiene la capacidad de almacenar OCR
        /// </summary>
        bool AlmacenaOCR { get; }

        /// <summary>
        /// Determin si las partes se acceden utilizando el identificador externo
        /// </summary>
        bool UtilizaIdentificadorExterno { get; }


        /// <summary>
        /// DEvulve la ruta al archivo generado
        /// </summary>
        /// <param name="version"></param>
        /// <param name="parteIds"></param>
        /// <returns></returns>
        Task<string> ObtieneZIP(Version version, List<string> parteIds);


        /// <summary>
        /// DEvulve la ruta al archivo generado
        /// </summary>
        /// <param name="version"></param>
        /// <param name="parteIds"></param>
        /// <returns></returns>
        Task<string> ObtienePDF(Version version, List<string> parteIds, int PorcientoEscala);

        /// <summary>
        /// Rota una imagen en un elemento de contenido
        /// </summary>
        /// <param name="ElementoId">elemento que contiene la parte</param>
        /// <param name="ParteId">Identificador de la parte</param>
        /// <param name="VersionId">Versión del elemento</param>
        /// <param name="VolumenId">Volumen del elemento</param>
        /// <param name="Extension">Extensión de la parte</param>
        /// <param name="Angulo">Ángulo a rotar</param>
        /// <returns></returns>
        Task<string> RotarImagen(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension, int Angulo);

        /// <summary>
        /// Refleja una imagen en un elemento de contenido
        /// </summary>
        /// <param name="ElementoId">elemento que contiene la parte</param>
        /// <param name="ParteId">Identificador de la parte</param>
        /// <param name="VersionId">Versión del elemento</param>
        /// <param name="VolumenId">Volumen del elemento</param>
        /// <param name="Extension">Extensión de la parte</param>
        /// <param name="Direccion">Dirección de refleho V o H</param>
        /// <returns></returns>
        Task<string> ReflejarImagen(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension, string Direccion);

    }
}
