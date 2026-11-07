using System;
using System.IO;

namespace SEM5_C_SistemaAruma
{
    // 1. CLASE PRODUCTO
    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
        public Producto Siguiente;

        public Producto(string codigo, string nombre, string categoria, double precio, int stock)
        {
            Codigo = codigo;
            Nombre = nombre;
            Categoria = categoria;
            Precio = precio;
            Stock = stock;
            Siguiente = null;
        }

        public override string ToString()
        {
            return $"[{Codigo}] {Nombre} | Categoría: {Categoria} | Precio: S/{Precio:F2} | Stock: {Stock} unid.";
        }
    }

    // 2. CLASE INVENTARIO
    public class Inventario
    {
        private Producto cabeza;

        public Inventario()
        {
            cabeza = null;
        }

        public void AgregarProducto(Producto nuevo)
        {
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                Producto actual = cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
        }

        public void MostrarTodos()
        {
            if (cabeza == null)
            {
                Console.WriteLine("El inventario está vacío.");
                return;
            }

            Producto actual = cabeza;
            while (actual != null)
            {
                Console.WriteLine(actual.ToString());
                actual = actual.Siguiente;
            }
        }

        public Producto BuscarPorCodigo(string codigo)
        {
            Producto actual = cabeza;
            while (actual != null)
            {
                if (actual.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return actual;
                }
                actual = actual.Siguiente;
            }
            return null;
        }

        public void BuscarPorNombreOCategoria(string criterio)
        {
            criterio = criterio.ToLower();
            Producto actual = cabeza;
            bool encontrado = false;

            while (actual != null)
            {
                if (actual.Nombre.ToLower().Contains(criterio) ||
                    actual.Categoria.ToLower().Contains(criterio))
                {
                    Console.WriteLine(actual.ToString());
                    encontrado = true;
                }
                actual = actual.Siguiente;
            }
            if (!encontrado)
            {
                Console.WriteLine("No se encontraron productos que coincidan con la búsqueda.");
            }
        }

        public bool EliminarPorCodigo(string codigo)
        {
            if (cabeza == null) return false;
            if (cabeza.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
            {
                cabeza = cabeza.Siguiente;
                return true;
            }

            Producto actual = cabeza;
            while (actual.Siguiente != null)
            {
                if (actual.Siguiente.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                {
                    actual.Siguiente = actual.Siguiente.Siguiente;
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }

        public void OrdenarPorPrecioBurbuja()
        {
            if (cabeza == null || cabeza.Siguiente == null) return;

            bool intercambiado;
            do
            {
                intercambiado = false;
                Producto actual = cabeza;

                while (actual.Siguiente != null)
                {
                    if (actual.Precio > actual.Siguiente.Precio)
                    {
                        string tempCod = actual.Codigo; string tempNom = actual.Nombre; string tempCat = actual.Categoria; double tempPre = actual.Precio; int tempSto = actual.Stock;

                        actual.Codigo = actual.Siguiente.Codigo;
                        actual.Nombre = actual.Siguiente.Nombre;
                        actual.Categoria = actual.Siguiente.Categoria;
                        actual.Precio = actual.Siguiente.Precio;
                        actual.Stock = actual.Siguiente.Stock;

                        actual.Siguiente.Codigo = tempCod;
                        actual.Siguiente.Nombre = tempNom;
                        actual.Siguiente.Categoria = tempCat;
                        actual.Siguiente.Precio = tempPre;
                        actual.Siguiente.Stock = tempSto;

                        intercambiado = true;
                    }
                    actual = actual.Siguiente;
                }
            } while (intercambiado);
        }
        public void OrdenarPorPrecioSeleccion()
        {
            if (cabeza == null || cabeza.Siguiente == null) return;

            Producto i = cabeza;
            while (i != null)
            {
                Producto min = i;
                Producto j = i.Siguiente;

                while (j != null)
                {
                    if (j.Precio < min.Precio)
                    {
                        min = j;
                    }
                    j = j.Siguiente;
                }

                if (min != i)
                {
                    string tempCod = i.Codigo; string tempNom = i.Nombre; string tempCat = i.Categoria; double tempPre = i.Precio; int tempSto = i.Stock;

                    i.Codigo = min.Codigo;
                    i.Nombre = min.Nombre;
                    i.Categoria = min.Categoria;
                    i.Precio = min.Precio;
                    i.Stock = min.Stock;

                    min.Codigo = tempCod;
                    min.Nombre = tempNom;
                    min.Categoria = tempCat;
                    min.Precio = tempPre;
                    min.Stock = tempSto;
                }
                i = i.Siguiente;
            }
        }
        public void GuardarEnArchivo(string ruta)
        {
            using (StreamWriter sw = new StreamWriter(ruta))
            {
                Producto actual = cabeza;
                while (actual != null)
                {
                    sw.WriteLine($"{actual.Codigo};{actual.Nombre};{actual.Categoria};{actual.Precio};{actual.Stock}");
                    actual = actual.Siguiente;
                }
            }
        }
        public void CargarDeArchivo(string ruta)
        {
            if (File.Exists(ruta))
            {
                cabeza = null;
                using (StreamReader sr = new StreamReader(ruta))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        string[] partes = linea.Split(';');
                        if (partes.Length == 5)
                        {
                            AgregarProducto(new Producto(partes[0], partes[1], partes[2], double.Parse(partes[3]), int.Parse(partes[4])));
                        }
                    }
                }
            }
        }
    }

    // 3. CLASE MENUSISTEMA
    public class MenuSistema
    {
        public static string GlobalNombreSistema = "Sistema de Gestión Aruma - Módulo Principal";

        private Inventario inventario;
        private string archivoDatos = "inventario_aruma.txt";

        public MenuSistema()
        {
            inventario = new Inventario();
            inventario.CargarDeArchivo(archivoDatos);
        }
        public bool ValidarYAplicarDescuento(ref double precioOriginal, double porcentajeDescuento)
        {
            if (porcentajeDescuento > 0 && porcentajeDescuento <= 100)
            {
                precioOriginal = precioOriginal - (precioOriginal * (porcentajeDescuento / 100.0));
                return true;
            }
            return false;
        }
        public void MostrarMenu()
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine($" {GlobalNombreSistema}");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Registrar nuevo producto");
                Console.WriteLine("2. Buscar producto (por Nombre o Categoría)");
                Console.WriteLine("3. Modificar producto");
                Console.WriteLine("4. Eliminar producto");
                Console.WriteLine("5. Mostrar inventario completo");
                Console.WriteLine("6. Registrar Venta (Actualiza stock)");
                Console.WriteLine("7. Ordenar productos por precio (Burbuja)");
                Console.WriteLine("8. Ordenar productos por precio (Selección)");
                Console.WriteLine("9. Guardar y Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                if (int.TryParse(Console.ReadLine(), out opcion))
                {
                    EjecutarOpcion(opcion);
                }
                else
                {
                    Console.WriteLine("Opción no válida. Presione una tecla...");
                    Console.ReadKey();
                }
            } while (opcion != 9);
        }
        private void EjecutarOpcion(int opcion)
        {
            Console.Clear();
            switch (opcion)
            {
                case 1:
                    RegistrarProducto();
                    break;
                case 2:
                    BuscarProducto();
                    break;
                case 3:
                    ModificarProducto();
                    break;
                case 4:
                    EliminarProducto();
                    break;
                case 5:
                    Console.WriteLine("--- INVENTARIO ACTUAL ---");
                    inventario.MostrarTodos();
                    break;
                case 6:
                    RegistrarVenta();
                    break;
                case 7:
                    Console.WriteLine("--- ORDENAMIENTO POR PRECIO (BURBUJA) ---");
                    inventario.OrdenarPorPrecioBurbuja();
                    inventario.MostrarTodos();
                    break;
                case 8:
                    Console.WriteLine("--- ORDENAMIENTO POR PRECIO (SELECCIÓN) ---");
                    inventario.OrdenarPorPrecioSeleccion();
                    inventario.MostrarTodos();
                    break;
                case 9:
                    inventario.GuardarEnArchivo(archivoDatos);
                    Console.WriteLine("Datos guardados. Saliendo...");
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }

            if (opcion != 9)
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
        private void RegistrarProducto()
        {
            Console.WriteLine("--- REGISTRAR NUEVO PRODUCTO ---");
            Console.Write("Código: ");
            string codigo = Console.ReadLine();
            if (inventario.BuscarPorCodigo(codigo) != null)
            {
                Console.WriteLine("Error: El código ya existe.");
                return;
            }
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Categoría: ");
            string categoria = Console.ReadLine();

            double precio;
            do { Console.Write("Precio (>0): "); } while (!double.TryParse(Console.ReadLine(), out precio) || precio <= 0);

            int stock;
            do { Console.Write("Stock (>=0): "); } while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0);

            inventario.AgregarProducto(new Producto(codigo, nombre, categoria, precio, stock));
            inventario.GuardarEnArchivo(archivoDatos);
            Console.WriteLine("¡Registrado con éxito!");
        }

        private void BuscarProducto()
        {
            Console.WriteLine("--- BUSCAR PRODUCTO ---");
            Console.Write("Ingrese nombre o categoría a buscar: ");
            string criterio = Console.ReadLine();
            inventario.BuscarPorNombreOCategoria(criterio);
        }
        private void ModificarProducto()
        {
            Console.WriteLine("--- MODIFICAR PRODUCTO ---");
            Console.Write("Código del producto a modificar: ");
            string codigo = Console.ReadLine();
            Producto p = inventario.BuscarPorCodigo(codigo);
            if (p == null) { Console.WriteLine("No encontrado."); return; }

            Console.WriteLine($"Actual: {p}");
            Console.Write("Nuevo nombre (Enter para omitir): ");
            string nom = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nom)) p.Nombre = nom;

            Console.Write("Nuevo precio (Enter para omitir): ");
            string preStr = Console.ReadLine();
            if (double.TryParse(preStr, out double pre) && pre > 0) p.Precio = pre;

            inventario.GuardarEnArchivo(archivoDatos);
            Console.WriteLine("¡Modificado con éxito!");
        }
        private void EliminarProducto()
        {
            Console.WriteLine("--- ELIMINAR PRODUCTO ---");
            Console.Write("Código del producto a eliminar: ");
            string codigo = Console.ReadLine();
            if (inventario.EliminarPorCodigo(codigo))
            {
                inventario.GuardarEnArchivo(archivoDatos);
                Console.WriteLine("Eliminado correctamente.");
            }
            else
            {
                Console.WriteLine("No se pudo eliminar o no existe.");
            }
        }
        private void RegistrarVenta()
        {
            Console.WriteLine("--- REGISTRAR VENTA ---");
            Console.Write("Ingrese el código del producto a vender: ");
            string codigo = Console.ReadLine();
            Producto prod = inventario.BuscarPorCodigo(codigo);
            if (prod == null)
            {
                Console.WriteLine("Producto no encontrado en el inventario.");
                return;
            }
            Console.WriteLine($"\nProducto encontrado: {prod.Nombre} | Stock disponible: {prod.Stock} | Precio: S/{prod.Precio:F2}");
            int cantidad;
            do
            {
                Console.Write("Ingrese la cantidad a vender (mayor a 0): ");
            } while (!int.TryParse(Console.ReadLine(), out cantidad) || cantidad <= 0);

            if (cantidad > prod.Stock)
            {
                Console.WriteLine("\nError: Stock insuficiente para realizar la venta.");
            }
            else
            {
                prod.Stock -= cantidad;
                inventario.GuardarEnArchivo(archivoDatos);

                Console.WriteLine("\n¡Venta registrada con éxito!");
                Console.WriteLine($"Total a cobrar: S/{(cantidad * prod.Precio):F2}");
                Console.WriteLine($"Nuevo stock de {prod.Nombre}: {prod.Stock} unid.");
            }
        }
    }

    // 4. CLASE PRINCIPAL
    class Program
    {
        static void Main(string[] args)
        {
            MenuSistema sistema = new MenuSistema();
            sistema.MostrarMenu();
        }
    }
}
