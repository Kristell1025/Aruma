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
        public Producto(string codigo, string nombre, string categoria, double precio, int stock)
        {
            Codigo = codigo;
            Nombre = nombre;
            Categoria = categoria;
            Precio = precio;
            Stock = stock;
        }
        public override string ToString()
        {
            return $"[{Codigo}] {Nombre} | Categoría: {Categoria} | Precio: S/{Precio:F2} | Stock: {Stock} unid.";
        }
    }
    // 2. CLASE NODO (LISTA ENLAZADA SIMPLE)
    public class Nodo
    {
        public Producto Datos { get; set; }
        public Nodo Siguiente { get; set; }
        public Nodo(Producto datos)
        {
            Datos = datos;
            Siguiente = null;
        }
    }
    // 3. CLASE LISTA ENLAZADA SIMPLE
    public class ListaEnlazada
    {
        private Nodo cabeza;
        public ListaEnlazada()
        {
            cabeza = null;
        }
        public void AgregarProducto(Producto producto)
        {
            Nodo nuevoNodo = new Nodo(producto);

            if (cabeza == null)
            {
                cabeza = nuevoNodo;
            }
            else
            {
                Nodo actual = cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }
        }
        public void MostrarTodos()
        {
            if (cabeza == null)
            {
                Console.WriteLine("El inventario está vacío.");
                return;
            }

            Nodo actual = cabeza;
            while (actual != null)
            {
                Console.WriteLine(actual.Datos.ToString());
                actual = actual.Siguiente;
            }
        }
        public void BuscarPorNombreOCategoria(string criterio)
        {
            criterio = criterio.ToLower();
            Nodo actual = cabeza;
            bool encontrado = false;

            while (actual != null)
            {
                if (actual.Datos.Nombre.ToLower().Contains(criterio) ||
                    actual.Datos.Categoria.ToLower().Contains(criterio))
                {
                    Console.WriteLine(actual.Datos.ToString());
                    encontrado = true;
                }
                actual = actual.Siguiente;
            }
            if (!encontrado)
            {
                Console.WriteLine("No se encontraron productos que coincidan con la búsqueda.");
            }
        }
        public Producto BuscarPorCodigo(string codigo)
        {
            Nodo actual = cabeza;

            while (actual != null)
            {
                if (actual.Datos.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Datos;
                }
                actual = actual.Siguiente;
            }
            return null;
        }

        // --- ELIMINAR PRODUCTO POR CÓDIGO ---
        public bool EliminarPorCodigo(string codigo)
        {
            if (cabeza == null) return false;
            if (cabeza.Datos.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
            {
                cabeza = cabeza.Siguiente;
                return true;
            }

            Nodo actual = cabeza;
            while (actual.Siguiente != null)
            {
                if (actual.Siguiente.Datos.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase))
                {
                    actual.Siguiente = actual.Siguiente.Siguiente;
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }
        // Ordenar por precio de menor a mayor (Método Burbuja)
        public void OrdenarPorPrecioBurbuja()
        {
            if (cabeza == null || cabeza.Siguiente == null)
            {
                Console.WriteLine("No hay suficientes productos para ordenar.");
                return;
            }
            bool intercambiado;
            do
            {
                intercambiado = false;
                Nodo actual = cabeza;

                while (actual.Siguiente != null)
                {
                    if (actual.Datos.Precio > actual.Siguiente.Datos.Precio)
                    {
                        Producto temporal = actual.Datos;
                        actual.Datos = actual.Siguiente.Datos;
                        actual.Siguiente.Datos = temporal;

                        intercambiado = true;
                    }
                    actual = actual.Siguiente;
                }
            } while (intercambiado);
            Console.WriteLine("Productos ordenados por precio (Método Burbuja).");
        }
        // --- ORDENAR POR PRECIO (MÉTODO SELECCIÓN) ---
        public void OrdenarPorPrecioSeleccion()
        {
            if (cabeza == null || cabeza.Siguiente == null)
            {
                Console.WriteLine("No hay suficientes productos para ordenar.");
                return;
            }
            Nodo i = cabeza;
            while (i != null)
            {
                Nodo min = i;
                Nodo j = i.Siguiente;

                while (j != null)
                {
                    if (j.Datos.Precio < min.Datos.Precio)
                    {
                        min = j;
                    }
                    j = j.Siguiente;
                }
                if (min != i)
                {
                    Producto temp = i.Datos;
                    i.Datos = min.Datos;
                    min.Datos = temp;
                }
                i = i.Siguiente;
            }
            Console.WriteLine("Productos ordenados por precio (Método Selección).");
        }
        public void GuardarEnArchivo(string ruta)
        {
            using (StreamWriter sw = new StreamWriter(ruta))
            {
                Nodo actual = cabeza;
                while (actual != null)
                {
                    sw.WriteLine($"{actual.Datos.Codigo};{actual.Datos.Nombre};{actual.Datos.Categoria};{actual.Datos.Precio};{actual.Datos.Stock}");
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
                            string codigo = partes[0];
                            string nombre = partes[1];
                            string categoria = partes[2];
                            double precio = double.Parse(partes[3]);
                            int stock = int.Parse(partes[4]);

                            AgregarProducto(new Producto(codigo, nombre, categoria, precio, stock));
                        }
                    }
                }
            }
        }
    }
    // 4. CLASE MENU
    public class Menu
    {
        private ListaEnlazada inventario;
        private string archivoDatos = "inventario_aruma.txt";
        public Menu()
        {
            inventario = new ListaEnlazada();
            inventario.CargarDeArchivo(archivoDatos);
        }
        public void MostrarMenu()
        {
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("     SISTEMA DE GESTIÓN - ARUMA         ");
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
                    Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
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
                    Console.WriteLine("--- INVENTARIO ACTUAL ARUMA ---");
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
                    Console.WriteLine("Datos guardados correctamente. Saliendo del sistema...");
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
            if (opcion != 9)
            {
                Console.WriteLine("\nPresione cualquier tecla para regresar al menú...");
                Console.ReadKey();
            }
        }
        private void RegistrarProducto()
        {
            Console.WriteLine("--- REGISTRAR NUEVO PRODUCTO ---");
            Console.Write("Código del producto: ");
            string codigo = Console.ReadLine();
            if (inventario.BuscarPorCodigo(codigo) != null)
            {
                Console.WriteLine("Error: Ya existe un producto registrado con ese código.");
                return;
            }
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Categoría: ");
            string categoria = Console.ReadLine();
            double precio;
            do
            {
                Console.Write("Precio (debe ser mayor a 0): S/");
            } while (!double.TryParse(Console.ReadLine(), out precio) || precio <= 0);

            int stock;
            do
            {
                Console.Write("Stock inicial (número entero positivo): ");
            } while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0);
            Producto nuevo = new Producto(codigo, nombre, categoria, precio, stock);
            inventario.AgregarProducto(nuevo);
            inventario.GuardarEnArchivo(archivoDatos);
            Console.WriteLine("\n¡Producto registrado con éxito!");
        }
        private void BuscarProducto()
        {
            Console.WriteLine("--- BUSCAR PRODUCTO ---");
            Console.Write("Ingrese el nombre o categoría que desea buscar: ");
            string criterio = Console.ReadLine();
            Console.WriteLine("\n--- Resultados de la búsqueda ---");
            inventario.BuscarPorNombreOCategoria(criterio);
        }
        // --- MÉTODO PARA MODIFICAR UN PRODUCTO ---
        private void ModificarProducto()
        {
            Console.WriteLine("--- MODIFICAR PRODUCTO ---");
            Console.Write("Ingrese el código del producto a modificar: ");
            string codigo = Console.ReadLine();
            Producto prod = inventario.BuscarPorCodigo(codigo);
            if (prod == null)
            {
                Console.WriteLine("Producto no encontrado.");
                return;
            }
            Console.WriteLine($"Actual -> {prod}");
            Console.WriteLine("\nIngrese los nuevos datos (deje en blanco o presione Enter para mantener el valor actual):");
            Console.Write($"Nuevo nombre [{prod.Nombre}]: ");
            string nombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nombre)) prod.Nombre = nombre;
            Console.Write($"Nueva categoría [{prod.Categoria}]: ");
            string categoria = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(categoria)) prod.Categoria = categoria;
            Console.Write($"Nuevo precio [S/{prod.Precio:F2}]: ");
            string precioStr = Console.ReadLine();
            if (double.TryParse(precioStr, out double precio) && precio > 0) prod.Precio = precio;
            Console.Write($"Nuevo stock [{prod.Stock}]: ");
            string stockStr = Console.ReadLine();
            if (int.TryParse(stockStr, out int stock) && stock >= 0) prod.Stock = stock;
            inventario.GuardarEnArchivo(archivoDatos);
            Console.WriteLine("\n¡Producto modificado con éxito!");
        }
        // --- MÉTODO PARA ELIMINAR UN PRODUCTO ---
        private void EliminarProducto()
        {
            Console.WriteLine("--- ELIMINAR PRODUCTO ---");
            Console.Write("Ingrese el código del producto que desea eliminar: ");
            string codigo = Console.ReadLine();
            Producto prod = inventario.BuscarPorCodigo(codigo);
            if (prod == null)
            {
                Console.WriteLine("Producto no encontrado en el inventario.");
                return;
            }
            Console.WriteLine($"Producto encontrado: {prod}");
            Console.Write("¿Está seguro de eliminar este producto? (S/N): ");
            string confirmacion = Console.ReadLine();
            if (confirmacion != null && confirmacion.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                bool eliminado = inventario.EliminarPorCodigo(codigo);
                if (eliminado)
                {
                    inventario.GuardarEnArchivo(archivoDatos);
                    Console.WriteLine("¡Producto eliminado correctamente del inventario!");
                }
                else
                {
                    Console.WriteLine("Error al intentar eliminar el producto.");
                }
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
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
    // 5. CLASE PRINCIPAL
    class Program
    {
        static void Main(string[] args)
        {
            Menu menuApp = new Menu();
            menuApp.MostrarMenu();
        }
    }
}
