using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.SampleMind
{

    public class BuscadorAmplitud : AbstractPathMind
    {

        List<NodoAmplitud> abierta = new List<NodoAmplitud>();

        NodoAmplitud nodo = new NodoAmplitud();

        BoardInfo board;

        bool isInit = false;

        NodoAmplitud nodoEncontrado;

        int index = 0;

        public GameObject personaje;



        public override void Repath()
        {

        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {

            if (nodoEncontrado == null)
            {
                return Locomotion.MoveDirection.None;
            }


            NodoAmplitud nodoTarget;

            index++;

            if (index < nodoEncontrado.getPadres().Count)
                nodoTarget = nodoEncontrado.getPadres()[index];
            else
                nodoTarget = nodoEncontrado;


            //Comparación de posiciones para realizar un movimiento:
            if ((nodoTarget.x == currentPos.ColumnId) && (nodoTarget.y > currentPos.RowId))
            {
                return Locomotion.MoveDirection.Up;
            }
            if ((nodoTarget.x == currentPos.ColumnId) && (nodoTarget.y < currentPos.RowId))
            {
                return Locomotion.MoveDirection.Down;
            }
            if ((nodoTarget.x < currentPos.ColumnId) && (nodoTarget.y == currentPos.RowId))
            {
                return Locomotion.MoveDirection.Left;
            }
            if ((nodoTarget.x > currentPos.ColumnId) && (nodoTarget.y == currentPos.RowId))
            {
                return Locomotion.MoveDirection.Right;
            }

            return Locomotion.MoveDirection.None;

        }

        public NodoAmplitud buscar()
        {
            //Inicia el tablero y la lista abierta, solo una vez
            if (!isInit)
            {
                board = (BoardInfo)character.BoardManager.boardInfo.Clone();

                nodo.init(board, null, (int)Math.Round(personaje.transform.position.x), (int)Math.Round(personaje.transform.position.y));

                abierta.Add(nodo);

                isInit = true;
            }
            

            if (abierta.Count == 0)
            {
                return null;
            }

            nodo = abierta[0];

            if (nodo.getIsMeta())
            {
                return nodo;
            }

            List<NodoAmplitud> hijos = nodo.Expandir();

            for (int i = 0; i < hijos.Count; i++)
            {
                abierta.Add(hijos[i]);
            }

            abierta.RemoveAt(0);

            return null;
        }



        void Update()
        {
            //Busca hasta que haya encontrado un nodo meta
            while (nodoEncontrado == null)
                nodoEncontrado = buscar();

        }
    }
}