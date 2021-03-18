using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Lexico
{
    class Lexic : Tokens, IDisposable
    {
        private StreamReader archivo;
        private StreamWriter bitacora;

        public Lexic()
        {
            Console.WriteLine("Compilando prueba.txt");

            /*Revisa que estén creados los archivos a analizar*/
            if (File.Exists("C:\\Archivos\\prueba.txt"))
            {
                archivo = new StreamReader("C:\\Archivos\\prueba.txt");
                bitacora = new StreamWriter("C:\\Archivos\\prueba.log");
                bitacora.AutoFlush = true;

                bitacora.WriteLine("Archivo: prueba.txt");
                bitacora.WriteLine("Directorio: C:\\Archivos");
            }
            else
            {
                /*Si no se encunetra, manda un mensaje por pantalla*/
                throw new Exception("El archivo prueba.txt no existe.");
            }

        }
        /*Destructor*/
        public void Dispose()
        {
            Console.WriteLine("Finaliza compilacion de prueba.txt");
            CerrarArchivos();
        }

        /*Metodo para cerrar los archivos despues de compilar*/
        private void CerrarArchivos()
        {
            archivo.Close();
            bitacora.Close();
        }

        /*Metodo para leer el archivo .txt*/
        public void NextToken()
        {
            char c;
            string palabra = "";

            /*Revisa los espacios vacios antes de tener algun dato*/
            while (char.IsWhiteSpace(c = (char)archivo.Read()))
            {

            }

            palabra += c;

            /*Revisa si el primer dato del archivo es una letra*/
            if (char.IsLetter(c))
            {
                setClasificacion(clasificaciones.identificador);

                /*Revisa los siguientes digitas hasta que no cumpla con caracteristicas de un identificador*/
                while (char.IsLetterOrDigit(c = (char)archivo.Peek()))
                {
                    palabra += c;
                    archivo.Read();
                }
            }

/*----------------------------------------Revisa si está abierta una cadena, ya sea con "" ó ''--------------------------*/
            else if (c == '"')
            {
                while (((c = (char)archivo.Peek()) != '"') && !FinDeArchivo())
                {
                    palabra += c;
                    archivo.Read();
                }
                palabra += c;
                archivo.Read();
                setClasificacion(clasificaciones.cadena);

                //if (((c = (char)archivo.Peek()) != '"') && FinDeArchivo())
                if (FinDeArchivo())
                {
                    throw new Exception("Error lexico: Falta cerrar \".");
                }

            }
            else if (c == '\'')
            {
                while (((c = (char)archivo.Peek()) != '\'') && !FinDeArchivo())
                {
                    palabra += c;
                    archivo.Read();
                }

                //if (((c = (char)archivo.Peek()) != '\'') && FinDeArchivo())
                if (FinDeArchivo())
                {
                    throw new Exception("Error lexico: Falta cerrar \'.");
                }

                palabra += c;
                archivo.Read();
                setClasificacion(clasificaciones.cadena);
            }
/*-------------------------------------------------------------------------------------------------------------------------*/

            /*Revisa si el dato es un numero*/
            else if (char.IsDigit(c))
            {
                setClasificacion(clasificaciones.numero);
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    palabra += c;
                    archivo.Read();
                }
                /*Revisa si se escribe un numero decimal de forma correcta*/
                if (c == '.')
                {
                    palabra += c;
                    archivo.Read();
                     /*Aquí revisa si hay numeros despues del punto para cumplir con la norma anterior*/
                    if (char.IsDigit(c = (char)archivo.Read()))
                    {
                        palabra += c;

                        while (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            palabra += c;
                            archivo.Read();
                        }
                    }

                    /*De no ser así, lanzara un mensaje por pantalla*/
                    else
                    {
                        throw new Exception("Error lexico: Se espera un digito.");
                    }
                }

                /*Revisa que esté escrito de manera correcta, otro tipo de numero*/
                if (char.ToLower(c) == 'e')
                {
                    palabra += c;
                    archivo.Read();

                    /*Aqui concatena operadores ternarios*/
                    if ((c = (char)archivo.Read()) == '+' || c == '-')
                    {
                        palabra += c;

                        c = (char)archivo.Read();
                    }
                    /*Revisa si se cumple con la norma de que debe existir un numero despues de
                     los operadores ternarios*/
                    if (char.IsDigit(c))
                    {
                        palabra += c;

                        while (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            palabra += c;
                            archivo.Read();
                        }
                    }
                    /*De no ser así, lanza un mensaje por pantalla*/
                    else
                    {
                        throw new Exception("Error lexico: Se espera un digito.");
                    }
                }
            }
            /*Revisa si el dato es un caracter :*/
            else if (c == ':')
            {
                setClasificacion(clasificaciones.caracter);

/*------------------De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion--------------------------*/
                if ((c = (char)archivo.Peek()) == '=')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.inicializacion);
                }
            }
            else if (c == '!')
            {
                setClasificacion(clasificaciones.operadorLogico);

                if ((c = (char)archivo.Peek()) == '=')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.operadorRelacional);
                }
            }
/*-----------------------------------------------------------------------------------------------------------------------------------*/

            /*Revisa si el dato que lee es un operador caracter*/
            else if (c == '&')
            {
                setClasificacion(clasificaciones.caracter);
                /*De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion*/
                if ((c = (char)archivo.Peek()) == '&')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.operadorLogico);
                }
            }

            /*Revisa si el dato que lee es un operador caracter */
            else if (c == '|')
            {
                setClasificacion(clasificaciones.caracter);
                /*De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion*/
                if ((c = (char)archivo.Peek()) == '|')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.operadorLogico);
                }
            }

            /*Revisa si el dato que lee es un operador relacional */
            else if (c == '<')
            {
                setClasificacion(clasificaciones.operadorRelacional);

                /*De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion*/
                if ((c = (char)archivo.Peek()) == '=' || c == '>')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.operadorRelacional);
                }
            }

            /*Revisa si el dato que lee es un operador relacional */
            else if ( c == '>')
            {
                setClasificacion(clasificaciones.operadorRelacional);

                /*De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion*/
                if ((c = (char)archivo.Peek()) == '=')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.operadorRelacional);
                }
            }

            /*Revisa si el dato que lee es un operador de termino */
            else if (c == '+')
            {
                setClasificacion(clasificaciones.operadorTermino);

                /*De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion*/
                if ((c = (char)archivo.Peek()) == '+' || c == '=')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.incrementoTermino);
                }
            }

            /*Revisa si el dato que lee es un operador de termino */
            else if (c == '-')
            {
                setClasificacion(clasificaciones.operadorTermino);

                /*De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion*/
                if ((c = (char)archivo.Peek()) == '-' || c == '=')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.incrementoTermino);
                }
            }

            /*Revisa si el dato que lee es un operador de factor */
            else if (c == '*' || c == '/' || c == '%')
            {
                setClasificacion(clasificaciones.operadorFactor);

                /*De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion*/
                if ((c = (char)archivo.Peek()) == '=')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.incrementoFactor);
                }
            }

            /*Revisa si el dato que lee es un operador dternario */
            else if (c == '?')
            {
                setClasificacion(clasificaciones.operadorTernario);
            }

            /*Revisa si el dato que lee es un fin de sentencia */
            else if (c == ';')
            {
                setClasificacion(clasificaciones.finSentencia);
            }

            /*Revisa si el dato que lee es un operador de asignacion */
            else if (c == '=')
            {
                setClasificacion(clasificaciones.asignacion);

                /*De ser así y está concatenado con otro operador, se le asigna una nueva clasificacion*/
                if ((c = (char)archivo.Peek()) == '=')
                {
                    palabra += c;
                    archivo.Read();

                    setClasificacion(clasificaciones.operadorRelacional);
                }
            }

            /*Si no es ninguno de los anteriores, el programa lo tomará directamente como un caracter*/
            else
            {
                setClasificacion(clasificaciones.caracter);
            }

            /*Una vez que termine de leer un caracter, los datos almacenados, que son
             el dato que se leyó, y el tipo de dato, lo escribirá en nuestra bitacora*/
            setContenido(palabra);
            bitacora.WriteLine("Token = " + getContenido());
            bitacora.WriteLine("Clasificacion = " + getClasificacion());
        }

        /*Cierra nuestro archivo de texto, después de la corrida*/
        public bool FinDeArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}