using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API
{

    /// <summary>
    /// Objeto estático para mantener el estado de la aplicaicón en ejeucuión
    /// </summary>
    public static class ServicioAplicacion
    {

        private static List<TipoAdministradorModulo> _ModulosAdministrados;

        /// <summary>
        /// Lista de módulos administrados del sistema
        /// </summary>
        public static List<TipoAdministradorModulo> ModulosAdministrados { get { return _ModulosAdministrados; } }

        /// <summary>
        /// Estableces los módulos administrados del sistema
        /// </summary>
        /// <param name="modulos"></param>
        public static void SetModulosAdministrador(List<TipoAdministradorModulo> modulos)
        {
            _ModulosAdministrados = modulos;
        }


        #region Plantillas y metadatos
        private static List<string> _PlantillasGeneradas;
        private static object LockPlantilla = new object();

        /// <summary>
        /// Identifica si  la plantilla ha sido generada, evita consultas adicionales al backend
        /// </summary>
        /// <param name="PlantilaId"></param>
        /// <returns></returns>
        public static bool EsPlantillaGenerada(string PlantilaId)
        {
            if (_PlantillasGeneradas == null) return false;

            return (_PlantillasGeneradas.IndexOf(PlantilaId) >= 0 ? true : false);
        }

        public static void AdicionaPlantillaGenerada(string PlantilaId) {
            lock (LockPlantilla)
            {
                if (_PlantillasGeneradas == null) _PlantillasGeneradas =new List<string>();
                if (_PlantillasGeneradas.IndexOf(PlantilaId) < 0) _PlantillasGeneradas.Add(PlantilaId);
            }
        }


        public static void EliminaPlantillaGenerada(string PlantilaId)
        {
            lock (LockPlantilla)
            {
                if (_PlantillasGeneradas == null) return;
                if (_PlantillasGeneradas.IndexOf(PlantilaId) >= 0) _PlantillasGeneradas.Remove(PlantilaId);
            }
        }


        #endregion










    }
}
