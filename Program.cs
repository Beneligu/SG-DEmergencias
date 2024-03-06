using System;

class Cola
{
    public class Nodo
    {
        public Paciente Dato;
        public Nodo Siguiente;
    }
    public Nodo frente;  
    private Nodo final;
    private int limite;
    
    public Cola(int limite)
    {
        this.limite = limite;
    }

    public void Encolar(Paciente paciente)
    {
        if (EstaLlena())
        {
            Console.WriteLine($"La cola está llena. No se puede encolar el paciente {paciente.Nombre}.");
            return;
        }

        Nodo nuevoNodo = new Nodo { Dato = paciente };

        if (frente == null)
        {
            frente = nuevoNodo;
            final = nuevoNodo;
        }
        else
        {
            final.Siguiente = nuevoNodo;
            final = nuevoNodo;
        }

        Console.WriteLine($"Se ha encolado un nuevo paciente. Prioridad: {paciente.Prioridad}");
    }

    public void Desencolar()
    {
        if (frente == null)
        {
            Console.WriteLine("La cola está vacía. No se puede atender paciente.");
            Console.Write("\n");
        }
        else
        {
            Nodo nodoAEliminar = frente;
            frente = frente.Siguiente;
            LiberarMemoria(nodoAEliminar);

            if (frente == null)
            {
                final = null;
            }
        }
    }

    private void LiberarMemoria(Nodo nodo)
    {
       nodo=null;
    }

    public void MostrarCola()
    {
        Nodo actual = frente;

        while (actual != null)
        {
            Console.Write($"{actual.Dato.Nombre} ({actual.Dato.Prioridad}) ");
            actual = actual.Siguiente;
        }

        Console.WriteLine();
    }

    private bool EstaLlena()
    {
        int contador = 0;
        Nodo actual = frente;

        while (actual != null)
        {
            contador++;
            actual = actual.Siguiente;
        }

        return contador >= limite;
    }

    public void DesencolarPorPrioridad()
    {
        if (frente == null)
        {
            Console.WriteLine("La cola está vacía. No se puede atender paciente.");
            Console.Write("\n");
            return;
        }

        Cola colaAuxiliar = new Cola(limite);
        Nodo nodoDesencolar = null;
        string[] prioridades = { "Accidentes aparatosos", "Infartos", "Afecciones respiratorias", "Partos" };

        for (int i = 0; i < prioridades.Length; i++)
        {
            string prioridadActual = prioridades[i];
            Nodo actual = frente;
            Nodo anterior = null;
            bool encontrado = false;

            while (actual != null)
            {
                if (actual.Dato.Prioridad == prioridadActual)
                {
                    nodoDesencolar = actual;
                    if (anterior == null)
                    {
                        frente = actual.Siguiente;
                    }
                    else
                    {
                        anterior.Siguiente = actual.Siguiente;
                    }
                    encontrado = true;
                    break;
                }

                Nodo siguiente = actual.Siguiente;
                if (siguiente != null)
                {
                    colaAuxiliar.Encolar(siguiente.Dato);
                }
                anterior = actual;
                actual = siguiente;
            }

            if (encontrado)
            {
                while (colaAuxiliar.frente != null)
                {
                    Encolar(colaAuxiliar.frente.Dato);
                    colaAuxiliar.Desencolar();
                }

                Console.WriteLine($"Se ha atendido el paciente {nodoDesencolar.Dato.Nombre} con prioridad {nodoDesencolar.Dato.Prioridad}.");
                LiberarMemoria(nodoDesencolar);
                colaAuxiliar = new Cola(limite); // Vaciar la colaAuxiliar
                return;
            }
            else
            {
                while (colaAuxiliar.frente != null)
                {
                    Encolar(colaAuxiliar.frente.Dato);
                    colaAuxiliar.Desencolar();
                }
                colaAuxiliar = new Cola(limite); // Vaciar la colaAuxiliar
            }
        }

        Console.WriteLine("No hay pacientes con prioridad.");
    }

    public int CantidadPacientes()
    {
        int contador = 0;
        Nodo actual = frente;

        while (actual != null)
        {
            contador++;
            actual = actual.Siguiente;
        }

        return contador;
    }
}

class Especialista
{
    public Cola ColaPacientesNormales { get; private set; }
    public Cola ColaPacientesPrioridad { get; private set; }
    private int limiteCola;

    public Especialista(int limiteCola)
    {
        ColaPacientesNormales = new Cola(limiteCola);
        ColaPacientesPrioridad = new Cola(limiteCola);
        this.limiteCola = limiteCola;
    }
}

class SistemaGestionEmergencias
{
    public Especialista especialistaNino;
    public Especialista especialistaAdulto;
    private Random random;

    public SistemaGestionEmergencias(int limiteCola)
    {
        especialistaNino = new Especialista(limiteCola);
        especialistaAdulto = new Especialista(limiteCola);
        random = new Random();
    }

    public void InsertarPaciente()
    {
        Console.Write("Ingrese el nombre del paciente: ");
        string nombre = Console.ReadLine();

        bool esPrioritario = random.Next(0, 2) == 0; // Genera aleatoriamente true o false
        string prioridad = esPrioritario ? GenerarPrioridadAleatoria() : "Normal";

        Paciente nuevoPaciente = new Paciente(nombre, esPrioritario, prioridad, random);

        if (nuevoPaciente.EsNiño)
        {
            AsignarPacienteAEspecialista(nuevoPaciente, especialistaNino);
Console.WriteLine($"Se ha ingresado un nuevo paciente niño. Nombre: {nombre}, Prioridad: {prioridad}\n");
        }
        else
        {
            AsignarPacienteAEspecialista(nuevoPaciente, especialistaAdulto);
Console.WriteLine($"Se ha ingresado un nuevo paciente adulto. Nombre: {nombre}, Prioridad: {prioridad}\n");
        }

    }

    private void AsignarPacienteAEspecialista(Paciente paciente, Especialista especialista)
    {
        if (paciente.EsPrioritario)
        {
            especialista.ColaPacientesPrioridad.Encolar(paciente);
        }
        else
        {
            especialista.ColaPacientesNormales.Encolar(paciente);
        }
    }

    private string GenerarPrioridadAleatoria()
    {
        string[] prioridades = { "Accidentes aparatosos", "Infartos", "Afecciones respiratorias", "Partos" };
        return prioridades[random.Next(prioridades.Length)];
    }

    public void MostrarColas()
    {
        Console.Write("\n");
        Console.WriteLine("----- Estado de las Colas -----");
        Console.WriteLine("Cola de Pacientes Niños Normales:");
        especialistaNino.ColaPacientesNormales.MostrarCola();
        Console.Write("\n");
        Console.WriteLine("Cola de Pacientes Niños con Prioridad:");
        especialistaNino.ColaPacientesPrioridad.MostrarCola();
        Console.Write("\n");
        Console.WriteLine("Cola de Pacientes Adultos Normales:");
        especialistaAdulto.ColaPacientesNormales.MostrarCola();
       Console.Write("\n");
        Console.WriteLine("Cola de Pacientes Adultos con Prioridad:");
        especialistaAdulto.ColaPacientesPrioridad.MostrarCola();
        Console.WriteLine("--------------------------------");
Console.Write("\n");
    }

    public void GenerarReporte()
    {
        int totalPacientesNino = especialistaNino.ColaPacientesNormales.CantidadPacientes() + especialistaNino.ColaPacientesPrioridad.CantidadPacientes();
        int totalPacientesAdulto = especialistaAdulto.ColaPacientesNormales.CantidadPacientes() + especialistaAdulto.ColaPacientesPrioridad.CantidadPacientes();

        Console.WriteLine("----- Reporte de Estadísticas -----");
        Console.WriteLine($"Total de pacientes en cola de Niños: {totalPacientesNino}");
        Console.WriteLine($"Total de pacientes en cola de Adultos: {totalPacientesAdulto}");

        MostrarEstadisticasPorPrioridad(especialistaNino.ColaPacientesPrioridad, "Niños");
        MostrarEstadisticasPorPrioridad(especialistaAdulto.ColaPacientesPrioridad, "Adultos");

        Console.WriteLine("-----------------------------------");
    }

    private void MostrarEstadisticasPorPrioridad(Cola colaPrioridad, string tipoPaciente)
    {
        string[] prioridades = { "Accidentes aparatosos", "Infartos", "Afecciones respiratorias", "Partos" };

        Console.WriteLine($"----- Prioridades de {tipoPaciente} -----");

        foreach (var prioridad in prioridades)
        {
            int cantidadPorPrioridad = ContarPacientesPorPrioridad(colaPrioridad, prioridad);
            Console.WriteLine($"{prioridad}: {cantidadPorPrioridad}");
        }
    }

    private int ContarPacientesPorPrioridad(Cola cola, string prioridad)
    {
        int contador = 0;
        Cola.Nodo actual = cola.frente;

        while (actual != null)
        {
            if (actual.Dato.Prioridad == prioridad)
            {
                contador++;
            }

            actual = actual.Siguiente;
        }

        return contador;
    }
}

class Paciente
{
    public string Nombre { get; private set; }
    public bool EsNiño { get; private set; }
    public bool EsPrioritario { get; private set; }
    public string Prioridad { get; private set; }

    public Paciente(string nombre, bool esPrioritario, string prioridad, Random random)
    {
        Nombre = nombre;
        EsNiño = GenerarEsNiñoAleatorio(random);
        EsPrioritario = esPrioritario;
        Prioridad = prioridad;
    }

    private bool GenerarEsNiñoAleatorio(Random random)
    {
        return random.Next(0, 2) == 0; // Genera aleatoriamente true o false
    }
}

class Program
{
    static void Main()
    {
        int limiteCola = 5; 
        SistemaGestionEmergencias sistema = new SistemaGestionEmergencias(limiteCola);

        while (true)
        {
            Console.WriteLine("----- Sistema de Gestión de Emergencias Médicas -----");
            Console.WriteLine("1. Insertar Paciente");
            Console.WriteLine("2. Remover Paciente");
            Console.WriteLine("3. Generar Reporte");
            Console.WriteLine("4. Mostrar Colas");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    sistema.InsertarPaciente();
                    break;

                case "2":
                    RemoverPaciente(sistema);
                    break;

                case "3":
                    Console.Write("\n");
                    sistema.GenerarReporte();
                    Console.Write("\n");
                    break;

                case "4":
                    sistema.MostrarColas();
                    break;

                case "5":
                    Console.WriteLine("Saliendo del sistema. ¡Hasta luego!");
                    return;

                default:
                    Console.WriteLine("Opción no válida. Por favor, seleccione una opción válida.");
                    break;
            }
        }
    }

    private static void RemoverPaciente(SistemaGestionEmergencias sistema)
    {
         Console.Write("\n");
         Console.WriteLine("Seleccione el tipo de paciente a remover:");
        Console.WriteLine("1. Paciente Niño");
        Console.WriteLine("2. Paciente Adulto");
        Console.Write("Ingrese el número de la opción: ");

        string tipoPaciente = Console.ReadLine();

        switch (tipoPaciente)
        {
            case "1":
                RemoverPacienteDeEspecialista(sistema.especialistaNino);
                break;

            case "2":
                RemoverPacienteDeEspecialista(sistema.especialistaAdulto);
                break;

            default:
                Console.WriteLine("Opción no válida. Selecciona 1 o 2.");
                break;
        }
    }

    public static void RemoverPacienteDeEspecialista(Especialista especialista)
    {
        Console.Write("\n");
        Console.WriteLine("Seleccione el tipo de cola:");
        Console.WriteLine("1. Pacientes Normales");
        Console.WriteLine("2. Pacientes con Prioridad");
        Console.Write("Ingrese el número de la opción: ");

        string tipoCola = Console.ReadLine();

        switch (tipoCola)
        {
            case "1":
                especialista.ColaPacientesNormales.Desencolar();
                break;

            case "2":
                especialista.ColaPacientesPrioridad.DesencolarPorPrioridad();
                break;

            default:
                Console.WriteLine("Opción no válida. Selecciona 1 o 2.");
                break;
        }
    }
}