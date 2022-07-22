using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PIKA.Servicio.Contenido.Gestores
{
    public static class JPEGLoseless
    {
        public static bool Rotar(string RutaJPEGTran, string Ruta, string Salida, int Angulo)
        {
            try
            {

                var info = new ProcessStartInfo
                {
                    FileName = RutaJPEGTran,
                    Arguments = $"-rotate {Angulo} {Ruta} {Salida}".TrimEnd(),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };


                using (var ps = Process.Start(info))
                {
                    ps.WaitForExit();

                    var exitCode = ps.ExitCode;

                    if (exitCode == 0)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"{ps.ExitCode} {ps.StandardOutput.ReadToEnd()}  {ps.StandardError.ReadToEnd()}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                throw;
            }
        }


        public static bool Reflejar(string RutaJPEGTran, string Ruta, string Salida, bool Horizontal)
        {
            try
            {
                string args = "";
                if(Horizontal)
                {
                    args = $"-flip horizontal {Ruta} {Salida}".TrimEnd();
                }
                else
                {
                    args = $"-flip vertical {Ruta} {Salida}".TrimEnd();
                }

                var info = new ProcessStartInfo
                {
                    FileName = RutaJPEGTran,
                    Arguments = args,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };


                using (var ps = Process.Start(info))
                {
                    ps.WaitForExit();

                    var exitCode = ps.ExitCode;

                    if (exitCode == 0)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"{ps.ExitCode} {ps.StandardOutput.ReadToEnd()}  {ps.StandardError.ReadToEnd()}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                throw;
            }
        }

    }
}
