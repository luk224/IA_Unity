using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;
using Assets.Scripts;
using System;

namespace Assets.Scripts.DataStructures
{

    public class NodoQ : ICloneable, IComparable<NodoQ>
    {
        NodoQ padre;
        CellInfo cell;
        List<NodoQ> padres = new List<NodoQ>();
        BoardInfo board;
        float coste;
        //Locomotion.MoveDirection direction;
        int direction;


        public int x { get; private set; }
        public int y { get; private set; }

        public void init(BoardInfo boardIn, List<NodoQ> padresIn, int i, int j)
        {
            padres = padresIn;
            x = i;
            y = j;
            board = boardIn;
            cell = board.CellInfos[x, y];

        }
        
        public List<NodoQ> Expandir()
        {

            List<NodoQ> hijos = new List<NodoQ>();
            CellInfo[] cellHijos = cell.WalkableNeighbours(board);
            List<NodoQ> padresIn = new List<NodoQ>();

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
                        encontrado = false;
                        if ((cellHijos[i].ColumnId == padresIn[j].x) && (cellHijos[i].RowId == padresIn[j].y))  // Si un antecesor es igual a un vecino, se descarta
                        {
                            encontrado = true;
                            break;
                           // j = padresIn.Count;
                        }
                    }
                    
                    if (!encontrado)
                    {
                        NodoQ hijo = new NodoQ();
                        hijo.init(this.board, padresIn, cellHijos[i].ColumnId, cellHijos[i].RowId);
                        hijo.setDirection(i);
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

        public List<NodoQ> getPadres()
        {
            return padres;
        }

        public float getCoste()
        {
            return coste;
        }

        public void setCoste(float costeIn)
        {
            coste = costeIn;
        }

        public int CompareTo(NodoQ other)
        {
            return this.getCoste().CompareTo(other.getCoste());
          
        }

        public void setDirection(int directionIn)
        {
            direction = directionIn;
        }

        public int getDirection()
        {
            return direction; ;
        }
    }

}