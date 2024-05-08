using System;

namespace MiProyecto
{
    class Program
    {
        static void Main()
        {
            //TypeClass.TypVar();
            //PrintTerminal.Print();
            Edad.MainEdad();
        }
    }
    class Edad
    {
        public static void MainEdad()
        {
            int edad = 8;
            bool esMayor = EsMayorDeEdad(edad);

            if (esMayor)
            {
                Console.WriteLine("la persona es mayor de edad");
            }
            else
            {
                Console.WriteLine("Es Menor");
            }
        }
        public static bool EsMayorDeEdad(int edad)
        {
            if (edad >= 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class TypeClass
    {
        public static void TypVar()
        {
            //vars/lets
            int NumeroEntero;
            double numberDecimal = 1.2;
            string cadenaTexto = "Texto de prueba";
            bool booleano = true;
            //asignar valores
            NumeroEntero = 23;
            //imprimir valores de las variables

            Console.WriteLine("int :" + NumeroEntero);
            Console.WriteLine("double:" + numberDecimal);
            Console.WriteLine("string: " + cadenaTexto);
            Console.WriteLine("boolean:" + booleano);
        }
    }
    class PrintTerminal
    {
        public static void Print()
        {
            Console.WriteLine("Holaaa");
        }
    }
}
