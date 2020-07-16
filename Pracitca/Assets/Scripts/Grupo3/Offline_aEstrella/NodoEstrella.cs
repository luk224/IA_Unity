using Assets.Scripts;
using Assets.Scripts.DataStructures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodoEstrella {

    public NodoEstrella papi; //nodo padre
    public CellInfo cell;
    public float g;
    public float hStar; //h*
    public float fStar; //f*
    public Locomotion.MoveDirection dir; //dirección para llegar a este nodo desde el padre

    
    public List<NodoEstrella> expandir(BoardInfo boardInfo, CellInfo goal)
    {
        List<NodoEstrella> aux = new List<NodoEstrella>();
        CellInfo[] vecinos = cell.WalkableNeighbours(boardInfo);

        for(int i = 0; i < vecinos.Length; i++)
        {
            if (vecinos[i] != null)
            {
                NodoEstrella nodoAux = new NodoEstrella();

                nodoAux.papi = this;

                if(i == 0)
                {
                    nodoAux.dir = Locomotion.MoveDirection.Up;
                }
                else if(i == 1)
                {
                    nodoAux.dir = Locomotion.MoveDirection.Right;
                }
                else if (i == 2)
                {
                    nodoAux.dir = Locomotion.MoveDirection.Down;
                }
                else if (i == 3)
                {
                    nodoAux.dir = Locomotion.MoveDirection.Left;
                }
                
                nodoAux.cell = vecinos[i];

                nodoAux.g = g + cell.WalkCost;
                nodoAux.hStar = (goal.ColumnId - nodoAux.cell.ColumnId) + (goal.RowId - nodoAux.cell.RowId); //distancia Manhattan
                nodoAux.fStar = nodoAux.g + nodoAux.hStar;
                aux.Add(nodoAux);
            }
        }
        
        return aux;
    }

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void setCell(CellInfo cell)
    {
        this.cell = cell;
    }
}
