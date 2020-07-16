using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.SampleMind
{
    public class BuscadorQ : AbstractPathMind
    {
        BoardInfo board;
        bool firstIsInit = false;
        
        NodoQ nodoEncontrado = new NodoQ();
        NodoQ meta = new NodoQ();
        NodoQ target = new NodoQ();
        
        float[,] tablaR; //Recompensas
        float[,] tablaQ; //Valores Q

        public float gamma = 0.8f;
        public float alpha = 0.3f;

        int vecesAprendidas = 0;

        public CharacterBehaviour personaje;
        
        int seed = 5000; //Semilla para los aleatorios
        
        public override void Repath()
        {
        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            seed++;

            UnityEngine.Random.seed = seed;
            
            float siguienteDireccion = 1;
            
            target.init(board, null, (int)Math.Round(personaje.transform.position.x), (int)Math.Round(personaje.transform.position.y));
            
            siguienteDireccion = buscarCamino(target);
           

            if (siguienteDireccion == 0)
            {
                return Locomotion.MoveDirection.Up;
            }
            if (siguienteDireccion == 1)
            {
                return Locomotion.MoveDirection.Right;
            }
            if (siguienteDireccion == 2)
            {
                return Locomotion.MoveDirection.Down;
            }
            if (siguienteDireccion == 3)
            {
                return Locomotion.MoveDirection.Left;
            }
            return Locomotion.MoveDirection.None;
            
        }

        
        public NodoQ aprender(NodoQ targetIn)
        {

            List<NodoQ> hijos = new List<NodoQ>();

            List<float> maxQ = new List<float>();
            
            hijos = targetIn.Expandir();

            //Elige un hijo aleatorio
            int hijoSiguiente = UnityEngine.Random.Range(0, hijos.Count);
            
            // Añadimos todas las acciones posibles del nodo siguiente
            for (int i = 0; i < 4; i++)
            {
                maxQ.Add(tablaQ[hijos[hijoSiguiente].y * board.NumColumns + hijos[hijoSiguiente].x, i]);
            }
            // Ordenamos todos los nodos
            maxQ.Sort();
            maxQ.Reverse();

            // Q(s,a) = (1 - alpha) * Q(s,a) + alpha * (R(s,a) + gamma * (maxQ))
            tablaQ[(targetIn.y * board.NumColumns) + targetIn.x, hijos[hijoSiguiente].getDirection()] = (1 - alpha) * tablaQ[(targetIn.y * board.NumColumns) + targetIn.x, hijos[hijoSiguiente].getDirection()] + (alpha) * (tablaR[(targetIn.y * board.NumColumns) + targetIn.x, hijos[hijoSiguiente].getDirection()] + gamma * maxQ[0]);
            
            return hijos[hijoSiguiente];
        }
        

        public float buscarCamino(NodoQ targetIn)
        {
            // En la posicion tenemos la Q, y en la segunda, la accion a realizar
            float[] maxQ = new float[2];
            maxQ[0] = 0;
            maxQ[1] = 0;

            // Añadimos todas las acciones posibles del nodo siguiente
            for (int i = 0; i < 4; i++)
            {
                if (maxQ[0] < tablaQ[targetIn.y * board.NumColumns + targetIn.x, i])
                {
                    maxQ[0] = tablaQ[targetIn.y * board.NumColumns + targetIn.x, i];
                    maxQ[1] = i;
                }
                else if (maxQ[0] == tablaQ[targetIn.y * board.NumColumns + targetIn.x, i])
                {
                    float randomEqual = UnityEngine.Random.Range(0.0f, 1.0f);
                    
                    if (randomEqual > 0.5f) //Ante celdas de igual Q, se eligen aleatoriamente
                    {
                        maxQ[0] = tablaQ[targetIn.y * board.NumColumns + targetIn.x, i];
                        maxQ[1] = i;
                    }
                }
            }

            return maxQ[1];
        }
        
        void Update()
        {
            if (!firstIsInit)
            {
                init();
                target.init(board, null, (int)Math.Round(personaje.transform.position.x), (int)Math.Round(personaje.transform.position.y));
            }


            while (vecesAprendidas < 1000)
            {

                seed++;
                UnityEngine.Random.seed = seed;

                nodoEncontrado = aprender(target);

                // Sí encontramos la meta, entonces ponemos al personaje en un lugar aleatorio
                if ((nodoEncontrado.x == meta.x) && (nodoEncontrado.y == meta.y))
                {
                    int xAleatoria = UnityEngine.Random.Range(0, board.NumColumns);
                    int yAleatoria = UnityEngine.Random.Range(0, board.NumRows);

                    target.init(board, null, xAleatoria, yAleatoria);

                    vecesAprendidas++;
                }

                target.init(board, null, nodoEncontrado.x, nodoEncontrado.y);

            }
        }


        public void init()
        {


            board = (BoardInfo)character.BoardManager.boardInfo.Clone();

            meta.init(board, null, board.Exit.ColumnId, board.Exit.RowId);


            //Expandimos los hijos de la meta
            List<NodoQ> hijosMeta = new List<NodoQ>();
            hijosMeta = meta.Expandir();

            // Inicializamos las tablas de recompensa y calidad, con el numero total de elementos, y el numero de acciones
            tablaR = new float[board.NumColumns * board.NumRows, 4];
            tablaQ = new float[board.NumColumns * board.NumRows, 4];

            // Por cada valor del array, inicializamos a 0
            // tablaQ[x, 0]: UP
            // tablaQ[x, 1]: RIGHT
            // tablaQ[x, 2]: DOWN
            // tablaQ[x, 3]: LEFT
            for (int i = 0; i < board.NumColumns * board.NumRows; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    tablaR[i, j] = 0;
                    tablaQ[i, j] = 0;
                }
            }

            // Localizamos cada uno de los hijos de la tabla, y a la accion que nos lleva a la meta, le ponemos recompensa 100
            for (int i = 0; i < hijosMeta.Count; i++)
            {
                if ((hijosMeta[i].x == meta.x) && (hijosMeta[i].y < meta.y))
                {
                    tablaR[board.NumColumns * hijosMeta[i].y + hijosMeta[i].x, 0] = 100;
                    Debug.Log("UP");
                }
                else if ((hijosMeta[i].x < meta.x) && (hijosMeta[i].y == meta.y))
                {
                    tablaR[board.NumColumns * hijosMeta[i].y + hijosMeta[i].x, 1] = 100;
                    Debug.Log("RIGHT");
                }
                else if ((hijosMeta[i].x == meta.x) && (hijosMeta[i].y > meta.y))
                {
                    tablaR[board.NumColumns * hijosMeta[i].y + hijosMeta[i].x, 2] = 100;
                    Debug.Log("DOWN");
                }
                else if ((hijosMeta[i].x > meta.x) && (hijosMeta[i].y == meta.y))
                {
                    tablaR[board.NumColumns * hijosMeta[i].y + hijosMeta[i].x, 3] = 100;
                    Debug.Log("LEFT");
                }
            }


            firstIsInit = true;

        }

        //Función que imprime la tabla de Q por consola
        public void printTable()
        {
            string print = "";

            for (int i = 0; i < board.NumColumns * board.NumRows; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    print += tablaQ[i, j];
                    print += "                     ";
                }
                print += "\n";
            }

            Debug.Log(print);

        }
    }
}