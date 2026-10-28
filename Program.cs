using System;
using System.Globalization;
using System.IO;
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        const string archivoVentas = "ventas.txt";
        var salir = false;


        do
        {
            Console.WriteLine("--- CAFETERÍA DOÑA MARTA ---");
            Console.WriteLine("1. Registrar venta");
            Console.WriteLine("2. Mostrar ventas");
            Console.WriteLine("3. Total del día");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out int opcion))
            {
                Console.WriteLine("Ingrese una opción numérica.");
                Console.WriteLine();
                continue;
            }

            switch (opcion)
            {
                case 1:
                    RegistrarVenta();
                    break;
                case 2:
                    MostrarVentas();
                    break;
                case 3:
                    MostrarTotal();
                    break;
                case 4:
                    Console.WriteLine("Hasta mañana, Doña Marta.");
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }

            Console.WriteLine();
        } while (!salir);

        void RegistrarVenta()
        {
            Console.Write("Producto: ");
            string producto = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(producto))
            {
                Console.WriteLine("El nombre es obligatorio.");
                return;
            }

            if (producto.Contains(';'))
            {
                Console.WriteLine("El nombre del producto no puede contener punto y coma.");
                return;
            }

            Console.Write("Monto: ");
            string textoMonto = Console.ReadLine() ?? string.Empty;

            if (!IntentarLeerMonto(textoMonto, out decimal monto) || monto <= 0)
            {
                Console.WriteLine("El monto debe ser un número mayor que cero.");
                return;
            }

            try
            {
                string linea = $"{producto.Trim()};{monto.ToString("0.00", CultureInfo.InvariantCulture)}";
                File.AppendAllText(archivoVentas, linea + Environment.NewLine);
                Console.WriteLine("Venta guardada.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"No se pudo guardar la venta: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"No hay permiso para guardar la venta: {ex.Message}");
            }
        }

        void MostrarVentas()
        {
            try
            {
                if (!File.Exists(archivoVentas))
                {
                    Console.WriteLine("Todavía no hay ventas.");
                    return;
                }

                string[] lineas = File.ReadAllLines(archivoVentas);
                int numeroVenta = 0;

                foreach (string linea in lineas)
                {
                    if (!IntentarLeerVenta(linea, out string producto, out decimal monto))
                    {
                        continue;
                    }

                    numeroVenta++;
                    Console.WriteLine($"{numeroVenta}. {producto} - ${monto.ToString("0.00", CultureInfo.InvariantCulture)}");
                }

                if (numeroVenta == 0)
                {
                    Console.WriteLine("Todavía no hay ventas.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"No se pudieron leer las ventas: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"No hay permiso para leer las ventas: {ex.Message}");
            }
        }

        void MostrarTotal()
        {
            try
            {
                if (!File.Exists(archivoVentas))
                {
                    Console.WriteLine("Todavía no hay ventas.");
                    return;
                }

                string[] lineas = File.ReadAllLines(archivoVentas);
                int cantidadVentas = 0;
                decimal total = 0;

                foreach (string linea in lineas)
                {
                    if (!IntentarLeerVenta(linea, out _, out decimal monto))
                    {
                        continue;
                    }

                    cantidadVentas++;
                    total += monto;
                }

                if (cantidadVentas == 0)
                {
                    Console.WriteLine("Todavía no hay ventas.");
                    return;
                }

                Console.WriteLine($"Ventas registradas: {cantidadVentas}");
                Console.WriteLine($"Total del día: ${total.ToString("0.00", CultureInfo.InvariantCulture)}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"No se pudieron calcular las ventas: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"No hay permiso para leer las ventas: {ex.Message}");
            }
        }

        bool IntentarLeerMonto(string texto, out decimal monto)
        {
            const NumberStyles estilos = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

            return decimal.TryParse(texto, estilos, CultureInfo.CurrentCulture, out monto)
                || decimal.TryParse(texto, estilos, CultureInfo.InvariantCulture, out monto);
        }

        bool IntentarLeerVenta(string linea, out string producto, out decimal monto)
        {
            producto = string.Empty;
            monto = 0;
            string[] partes = linea.Split(';');

            if (partes.Length != 2 || string.IsNullOrWhiteSpace(partes[0]) || !IntentarLeerMonto(partes[1], out monto) || monto <= 0)
            {
                return false;
            }

            producto = partes[0];
            return true;
        }
    }
}