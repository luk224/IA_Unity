using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;
using Assets.Scripts;
using System;

namespace Assets.Scripts.DataStructures
{

    public class NodoAmplitud : ICloneable
    {

        NodoAmplitud padre;
        CellInfo cell;
        List<NodoAmplitud> padres = new List<NodoAmplitud>();
        BoardInfo board;
        Locomotion.MoveDirection direction;


        public int x { get; private set; }
        public int y { get; private set; }

        public void init(BoardInfo boardIn, List<NodoAmplitud> padresIn, int i, int j)
        {
            padres = padresIn;
            x = i;
            y = j;
            board = boardIn;
            cell = board.CellInfos[x, y];
        }
        
        public List<NodoAmplitud> Expandir()
        {
            List<NodoAmplitud> hijos = new List<NodoAmplitud>();
            CellInfo[] cellHijos = cell.WalkableNeighbours(board);
            List<NodoAmplitud> padresIn = new List<NodoAmplitud>();

                
            bool encontrado = false;

            if (padres != null)
            {
                for (int i = 0; i < padres.Count; i++)
                {
                    padresIn.Add(padres[i]);
                }
            }
            
            padresIn.Add(this);
          

            for (int i = 0; i < cellHijos.Length; i++)
            {
                if (cellHijos[i] != null)
                {
                    for (int j = 0; j < padresIn.Count; j++)             //Evalúa cada uno de los padres
                    {
                        if ((cellHijos[i].ColumnId == padresIn[j].x) && (cellHijos[i].RowId == padresIn[j].y))  // Si un antecesor es igual a un vecino, se descarta
                        {
                            encontrado = true;
                            break;
                            //j = padresIn.Count;
                        }
                    }
                    
                    if (!encontrado)
                    {
                        NodoAmplitud hijo = new NodoAmplitud();
                        hijo.init(this.board, padresIn, cellHijos[i].ColumnId, cellHijos[i].RowId);
                        hijos.Add(hijo);
                    }
                
                }
            }
            return hijos;
        }


        public bool getIsMeta()
        {
            CellInfo meta = board.Exit;
            return (meta.ColumnId == x && meta.RowId == y);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public List<NodoAmplitud> getPadres()
        {
            return padres;
        }
    }

}