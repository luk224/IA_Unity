using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;
using Assets.Scripts;
using System;

namespace Assets.Scripts.DataStructures
{

    public class NodoOnline : ICloneable, IComparable<NodoOnline>
    
    {

        NodoOnline padre;
        CellInfo cell;
        List<NodoOnline> padres = new List<NodoOnline>();
        BoardInfo board;
        float coste;
        //Locomotion.MoveDirection direction;
        int direction;


        public int x { get; private set; }
        public int y { get; private set; }

        public void init(BoardInfo boardIn, List<NodoOnline> padresIn, int i, int j)
        {

            
            padres = padresIn;
            x = i;
            y = j;
            board = boardIn;
            cell = board.CellInfos[x, y];

        }

        // Use this for initialization
        public List<NodoOnline> Expandir()
        {

            List<NodoOnline> hijos = new List<NodoOnline>();
            CellInfo[] cellHijos = cell.WalkableNeighbours(board);
            List<NodoOnline> padresIn = new List<NodoOnline>();

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
                    
                        NodoOnline hijo = new NodoOnline();
                        hijo.init(this.board, padresIn, cellHijos[i].ColumnId, cellHijos[i].RowId);
                        hijo.setDirection(i);
                        hijos.Add(hijo);
                    
                
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

        public List<NodoOnline> getPadres()
        {
            return padres;
        }

        public void setPadres(List<NodoOnline> padresIn)
        {
            padres = padresIn;
        }


        public float getCoste()
        {
            return coste;
        }

        public void setCoste(float costeIn)
        {
            coste = costeIn;
        }

        public int CompareTo(NodoOnline other)
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