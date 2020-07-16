using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.SampleMind
{

    public class BuscadorOnline : AbstractPathMind
    {
        BoardInfo board;

        bool firstIsInit = false;

        NodoOnline nodoEncontrado;

        NodoOnline meta = new NodoOnline();

        public EnemyBehaviour[] enemies;

        public GameObject personaje;

        public int profundidad = 0;



        public override void Repath()
        {

        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            nodoEncontrado = null;

            if (!firstIsInit)
            {
                init();
            }

            enemies = FindObjectsOfType<EnemyBehaviour>();


       
            if (enemies.Length == 0)
                nodoEncontrado = buscar(meta);
            else
                nodoEncontrado = buscar(getEnemy(currentPos));


            

            if ((nodoEncontrado.x == currentPos.ColumnId) && (nodoEncontrado.y > currentPos.RowId))
            {
                return Locomotion.MoveDirection.Up;
            }
            if ((nodoEncontrado.x == currentPos.ColumnId) && (nodoEncontrado.y < currentPos.RowId))
            {
                return Locomotion.MoveDirection.Down;
            }
            if ((nodoEncontrado.x < currentPos.ColumnId) && (nodoEncontrado.y == currentPos.RowId))
            {
                return Locomotion.MoveDirection.Left;
            }
            if ((nodoEncontrado.x > currentPos.ColumnId) && (nodoEncontrado.y == currentPos.RowId))
            {
                return Locomotion.MoveDirection.Right;
            }


            return Locomotion.MoveDirection.None;
        }



        public NodoOnline buscar(NodoOnline enemy)
        {
            List<NodoOnline> abierta = new List<NodoOnline>();

            List<NodoOnline> hijos = new List<NodoOnline>();

            NodoOnline nodoPersonaje = new NodoOnline();
                        
            nodoPersonaje.init(board, null, (int)Math.Round(personaje.transform.position.x), (int)Math.Round(personaje.transform.position.y));
            nodoPersonaje.setCoste(Mathf.Sqrt(Mathf.Pow(personaje.transform.position.x - enemy.x, 2) + Mathf.Pow(personaje.transform.position.y - enemy.y, 2)));

            abierta.Add(nodoPersonaje);

            
            while (true)
            {

                hijos = abierta[0].Expandir();

                if (hijos[hijos.Count - 1].getPadres().Count > profundidad)
                {
                    break;
                }

                for (int i = 0; i < hijos.Count; i++)
                {
                    if ((enemies.Length == 0) || (hijos[i].x != meta.x || hijos[i].y != meta.y) && (enemies.Length > 0))
                    {

                        hijos[i].setCoste(Mathf.Sqrt(Mathf.Pow(hijos[i].x - enemy.x, 2) + Mathf.Pow(hijos[i].y - enemy.y, 2)));

                        abierta.Add(hijos[i]);

                        //Si encontramos al nodo enemigo, lo devolvemos:
                        if ((hijos[i].x == enemy.x) && (hijos[i].y == enemy.y))
                        {
                            if (hijos[i].getPadres().Count == 2)
                                return hijos[i].getPadres()[1];               //Como profundidad es k=2, devolvemos el padre inmediato del nodo del enemigo

                            else if (hijos[i].getPadres().Count == 1)
                                return hijos[i];                              //Como la profundidad es k=1, devolvemos el hijo inmediato del personaje

                        }
                    }

                }
                abierta.RemoveAt(0);
            }

            abierta.Sort();
            
            return abierta[0].getPadres()[1];
        }






        public NodoOnline getEnemy(CellInfo currentPos)
        {
            List<NodoOnline> enemyNodos = new List<NodoOnline>();


            //Saca un nodo por cada enemigo
            for (int i = 0; i < enemies.Length; i++)
            {
                NodoOnline enemyNodo = new NodoOnline();
                //Creamos el nodo con los datos del enemigo
                enemyNodo.init(board, null, enemies[i].CurrentPosition().ColumnId, enemies[i].CurrentPosition().RowId);
                enemyNodo.setCoste(Mathf.Sqrt(Mathf.Pow(enemyNodo.x - currentPos.ColumnId, 2) + Mathf.Pow(enemyNodo.y - currentPos.RowId, 2)));
                
                enemyNodos.Add(enemyNodo);
            }

            enemyNodos.Sort();
            return enemyNodos[0];
        }


        public void init()
        {
            board = (BoardInfo)character.BoardManager.boardInfo.Clone();

            enemies = FindObjectsOfType<EnemyBehaviour>();

            meta.init(board, null, board.Exit.ColumnId, board.Exit.RowId);

            firstIsInit = true;
        }


        void Update()
        {

        }
    }
}