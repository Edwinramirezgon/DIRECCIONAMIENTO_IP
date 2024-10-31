using System;
using System.IO;

class Program
{
    const string rutaArchivo = "resultado_IPs.txt";

    static void Main()
    {
        string opcion;

        // Muestra el contenido del archivo en formato de tabla al inicio del programa
        MostrarArchivoEnTabla();

        do
        {
            Console.Write("INGRESE LA DIRECCION IP QUE DESEA CONSULTAR: ");
            string DireccionIp = Console.ReadLine();

            if (IpValida(DireccionIp))
            {
                string clase = DeterminarClase(DireccionIp);
                string tipo = DeterminarTipo(DireccionIp, clase);
                string estructura = DeterminarEstructura(clase);
                string direccionRed = DeterminarDRed(DireccionIp, clase);
                string broadcast = DeterminarBroadcast(direccionRed, clase);
                string mascara = DeterminarMascara(clase);
                string hosts = DeterminarHosts(DireccionIp, clase);

                Console.WriteLine("\nResultado:");
                Console.WriteLine($"IP: {DireccionIp}");
                Console.WriteLine($"Clase: {clase}");
                Console.WriteLine($"Tipo: {tipo}");
                Console.WriteLine($"Estructura: {estructura}");
                Console.WriteLine($"Dirección de Red: {direccionRed}");
                Console.WriteLine($"Broadcast: {broadcast}");
                Console.WriteLine($"Máscara Por Defecto: {mascara}");
                Console.WriteLine($"Direccion De Hosts: {hosts}");

                // Guarda en archivo
                GuardarEnArchivo(DireccionIp, clase, tipo, estructura, direccionRed, broadcast, mascara, hosts);

                // Muestra el contenido actualizado del archivo
                MostrarArchivoEnTabla();
            }
            else
            {
                Console.WriteLine("La dirección IP ingresada no es válida.");
            }

            Console.Write("\n¿Desea procesar otra IP? (S/N): ");
            opcion = Console.ReadLine().ToUpper();

        } while (opcion == "S");

        Console.WriteLine("Gracias Por Utilizar El Programa.");
    }

    static void MostrarArchivoEnTabla()
    {
        if (File.Exists(rutaArchivo))
        {
            Console.WriteLine("\nContenido del archivo en formato de tabla:\n");
            Console.WriteLine(File.ReadAllText(rutaArchivo));
        }
        else
        {
            Console.WriteLine("\nEl archivo de resultados está vacío o no existe aún.");
        }
    }

    static bool IpValida(string DireccionIp)
    {
        var partes = DireccionIp.Split('.');
        if (partes.Length != 4) return false;

        foreach (var parte in partes)
        {
            if (!int.TryParse(parte, out int num) || num < 0 || num > 255)
                return false;
        }
        return true;
    }

    static string DeterminarClase(string DireccionIp)
    {
        byte PrimerOcteto = byte.Parse(DireccionIp.Split('.')[0]);
        if (PrimerOcteto >= 0 && PrimerOcteto <= 127) return "A";
        else if (PrimerOcteto >= 128 && PrimerOcteto <= 191) return "B";
        else if (PrimerOcteto >= 192 && PrimerOcteto <= 223) return "C";
        else if (PrimerOcteto >= 224 && PrimerOcteto <= 239) return "D";
        else return "E";
    }

    static string DeterminarTipo(string DireccionIp, string clase)
    {
        var octetos = DireccionIp.Split('.');

        int primerOcteto = int.Parse(octetos[0]);
        int segundoOcteto = int.Parse(octetos[1]);
        int tercerOcteto = int.Parse(octetos[2]);
        int cuartoOcteto = int.Parse(octetos[3]);

        if (primerOcteto == 127 ||
           primerOcteto == 0 ||
           (primerOcteto >= 224 && primerOcteto <= 255) ||
           (clase.Equals("A") && segundoOcteto == 0 && tercerOcteto == 0 && cuartoOcteto == 0) ||
           (clase.Equals("A") && segundoOcteto == 255 && tercerOcteto == 255 && cuartoOcteto == 255) ||
           (clase.Equals("B") && tercerOcteto == 0 && cuartoOcteto == 0) ||
           (clase.Equals("B") && tercerOcteto == 255 && cuartoOcteto == 255) ||
           (clase.Equals("C") && cuartoOcteto == 0) ||
           (clase.Equals("C") && cuartoOcteto == 255))
        {
            return "RESERVADA";
        }
        else if (DireccionIp.StartsWith("10.") ||
            DireccionIp.StartsWith("192.168.") ||
            (DireccionIp.StartsWith("172.") && segundoOcteto >= 16 && segundoOcteto <= 31))
        {
            return "PRIVADA";
        }
        else
        {
            return "PUBLICA";
        }
    }

    static string DeterminarEstructura(string clase)
    {
        return clase switch
        {
            "A" => "N.H.H.H",
            "B" => "N.N.H.H",
            "C" => "N.N.N.H",
            _ => "-------"
        };
    }

    static string DeterminarDRed(string DireccionIp, string clase)
    {
        var Octetos = DireccionIp.Split('.');
        return clase switch
        {
            "A" => $"{Octetos[0]}.0.0.0",
            "B" => $"{Octetos[0]}.{Octetos[1]}.0.0",
            "C" => $"{Octetos[0]}.{Octetos[1]}.{Octetos[2]}.0",
            _ => "--------"
        };
    }

    static string DeterminarBroadcast(string DireccionDeRed, string clase)
    {
        var Octetos = DireccionDeRed.Split('.');
        return clase switch
        {
            "A" => $"{Octetos[0]}.255.255.255",
            "B" => $"{Octetos[0]}.{Octetos[1]}.255.255",
            "C" => $"{Octetos[0]}.{Octetos[1]}.{Octetos[2]}.255",
            _ => "--------"
        };
    }

    static string DeterminarMascara(string clase)
    {
        return clase switch
        {
            "A" => "255.0.0.0",
            "B" => "255.255.0.0",
            "C" => "255.255.255.0",
            _ => "--------"
        };
    }

    static string DeterminarHosts(string DireccionIp, string clase)
    {
        var Octetos = DireccionIp.Split('.');
        return clase switch
        {
            "A" => $"0.{Octetos[1]}.{Octetos[2]}.{Octetos[3]}",
            "B" => $"0.0.{Octetos[2]}.{Octetos[3]}",
            "C" => $"0.0.0.{Octetos[3]}",
            _ => "--------"
        };
    }

    static void GuardarEnArchivo(string DireccionIp, string clase, string tipo, string estructura, string direccionRed, string broadcast, string mascara, string hosts)
    {
        string contenido = $"| {DireccionIp,-15} | {clase,-5} | {tipo,-10} | {estructura,-10} | {direccionRed,-15} | {broadcast,-15} | {mascara,-15} | {hosts,-15} |\n";

        if (!File.Exists(rutaArchivo) || new FileInfo(rutaArchivo).Length == 0)
        {
            string encabezado = $"| {"IP",-15} | {"Clase",-5} | {"Tipo",-10} | {"Estructura",-10} | {"Dirección Red",-15} | {"Broadcast",-15} | {"Máscara",-15} | {"Hosts",-15} |\n";
            encabezado += new string('-', encabezado.Length) + "\n";
            File.AppendAllText(rutaArchivo, encabezado);
        }

        File.AppendAllText(rutaArchivo, contenido);
    }
}