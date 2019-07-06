using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablero : MonoBehaviour
{
    private bool inGame = true; // Este booleano permite iniciar y detener el juego cuando hay un ganador
    private bool turnoJugador1; // Este booleano permite intercambiar los turnos entre dos jugadores.

	// Los enteros para ancho y alto, son los que asignan la dimensión del tablero.
    int ancho = 10;
    int alto = 10;

    public GameObject pieza;
    private GameObject[,] esf; // Esta matriz de GameObject representa las posiciones de cada esfera del tablero.

	// Los colores fueron declarados como públicos para poder asignarlos desde el inspector.
    public Color baseColor;
    public Color player1;
    public Color player2;

    public void Start() // Este bloque de código se encarga de generar las esferas que componen el tablero.
    {
        esf = new GameObject[ancho, alto];
        for (int i = 0; i < ancho; i++)
        {
            for (int j = 0; j < alto; j++)
            {
                GameObject esfera = GameObject.Instantiate(pieza) as GameObject;
                Vector3 position = new Vector3(i, j, 0);
                esfera.transform.position = position;

                esfera.GetComponent<Renderer>().material.color = baseColor;

                esf[i, j] = esfera;
            }
        }
    }

    public void Update() // Este bloque de código es el encargado de recibir la ubicación en el campo de juego del mouse.
    {
        if (inGame == true) // El juego es oficialmente iniciado, solamente si el booleano inGame es verdadero.
        {
            Vector3 mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SeleccionarFicha(mPosition);
        }
    }

	/* 
	Esta función se encarga de recibir las coordenadas en X y Y del mouse, además del click, que son las que permiten
	identificar la ficha que ha seleccionado el jugador.
	*/
    public void SeleccionarFicha(Vector3 position) 
    {
        int i = (int)(position.x + 0.5f);
        int j = (int)(position.y + 0.5f);

        if (Input.GetButtonDown("Fire1"))
        {
            if (i >= 0 && j >= 0 && i < ancho && j < alto)
            {
                GameObject esfera = esf[i, j];
                if (esfera.GetComponent<Renderer>().material.color == baseColor)
                {
                    Color colorAUsar = Color.clear;
                    if (turnoJugador1)
                        colorAUsar = player1;
                    else
                        colorAUsar = player2;
                    esfera.GetComponent<Renderer>().material.color = colorAUsar;
                    turnoJugador1 = !turnoJugador1;

					// A continuación se llaman todas las funciones encargadas de verificar cuando hay o no un ganador, revisando en todas las direcciones posibles.

                    RevisionX(i, j, colorAUsar);
                    RevisionY(i, j, colorAUsar);
                    RevisionDiagonal1(i, j, colorAUsar);
                    RevisionDiagonal2(i, j, colorAUsar);
                    RevisionTetris(i, j, colorAUsar);
                }
            }
        }
    }

	/* 
	Esta función, utilizando un contador, revisa horizontalmente cuando hay fichas en línea.
	En caso contrario, el contador se reinicia.
	*/
    public void RevisionX(int x, int y, Color colorAVerificar)
    {
        int contador = 0;
        for (int i = x - 3; i <= x + 3; i++) 
        {
            if (i < 0 || i >= ancho) // Este For, es el encargado de que no exceda el ancho del tablero.
                continue; // El Continue permite saltar u omitir las sentencias restantes y continuar con la siguiente.

            GameObject esfera = esf[i, y];

            if (esfera.GetComponent<Renderer>().material.color == colorAVerificar)
            {
                contador++;
                if (contador == 4)
                {
                    Debug.Log("Ganador");
                    inGame = false;
                }
            }
            else
                contador = 0;
        }
    }

	// Esta función revisa verticalmente el tablero, en caso de que no haya fichas en línea el contador se reinicia. 
    public void RevisionY(int x, int y, Color colorAVerificar)
    {
        int contador = 0;
        for (int j = y - 3; j <= y + 3; j++) 
        {
            if (j < 0 || j >= alto) // Este For, es el encargado de que no exceda el alto del tablero.
                continue;

            GameObject esfera = esf[x, j];

            if (esfera.GetComponent<Renderer>().material.color == colorAVerificar)
            {
                contador++;
                if (contador == 4)
                {
                    Debug.Log("Ganador");
                    inGame = false;
                }
            }
            else
                contador = 0;
        }
    }

	/*
	Las siguientes dos funciones, se encargan de revisar en dirección diagonal el tablero.
	Puesto que era necesario tener proporcionalidad directa o inversa en ambos ejes, se utilizaron tanto los valores
	de X como los de Y.
	 * "j" está siendo utilizado como Y, e "i" como X.
	 */
    public void RevisionDiagonal1(int x, int y, Color colorAVerificar)
    {

        int contador = 0;
        for (int i = x - 4; i <= x + 2;)
        {
            for (int j = y - 4; j <= y + 2;)
            {
                i++;
                j++;

                if (j < 0 || j >= alto || i < 0 || i >= ancho)
                    continue;
                GameObject esfera = esf[i, j];

                if (esfera.GetComponent<Renderer>().material.color == colorAVerificar)
                {
                    contador++;
                    if (contador == 4)
                    {
                        Debug.Log("Ganador");
                        inGame = false;
                    }
                }
                else
                {
                    contador = 0;
                }
            }

        }
    }

	// Se hicieron dos revisiones para poder abarcar las dos direcciones posibles.
    public void RevisionDiagonal2(int x, int y, Color colorAVerificar)
    {
        int contador = 0;
        for (int i = x - 4; i <= x + 2;)
        {
            for (int j = y + 4; j >= y - 2;)
            {
                i++;
                j--;

                if (j < 0 || j >= alto || i < 0 || i >= ancho)
                    continue;

                GameObject esfera = esf[i, j];

                if (esfera.GetComponent<Renderer>().material.color == colorAVerificar)
                {
                    contador++;

                    if (contador == 4)
                    {
                        Debug.Log("Ganador");
                        inGame = false;
                    }
                }
                else
                {
                    contador = 0;
                }
            }

        }
    }

	/*
	La revisión de Tetris, siendo mi regla propia, es la encargada de revisar el tablero, buscando cuatro 
	fichas en línea pero formando una L. Fue necesario utilizar tanto los valores de Y, como los de X. 
	*/
    public void RevisionTetris(int x, int y, Color colorAVerificar)
    {
        int i = x - 1;
		int i2 = x + 1;
		int contador = 0;

		for(int j = y + 2; j >= y; j--)
		{
			if (j < 0 || j >= alto || i < 0 || i >= ancho)
		    return; // Este Return cancela en caso de que se salga de la dimensión del tablero.

		    GameObject esfera = esf[i, j];
			if (esfera.GetComponent<Renderer>().material.color != colorAVerificar)
			{
				return; // Revisa que sea el mismo color a verificar.
			}

			if(i >= 0 && i < ancho && j >= 0 && j < alto)
			{
			    contador++;
			}

			if(i2 >= 0 && i2 < ancho && j >= 0 && j < alto)
			{
			    contador++;
			}

			else
			{
				contador = 0;
			}

			if(contador == 4)
			{
				Debug.Log("Get in Loser");
				inGame = false;
			}
		}
    }
}


