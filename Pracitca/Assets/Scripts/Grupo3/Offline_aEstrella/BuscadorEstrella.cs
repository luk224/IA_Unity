using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DataStructures;
using System.Linq;
using System;

public class BuscadorEstrella : AbstractPathMind {

    List<NodoEstrella> abierta = new List<NodoEstrella>();

    NodoEstrella nodoActual;

    NodoEstrella nodoMeta;

    bool caminoEncontrado = false;

    Stack<Locomotion.MoveDirection> pasos = new Stack<Locomotion.MoveDirection>(); //pila de pasos (dir) a seguir

    void Start()
    {
    }

    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        if (!caminoEncontrado) {
            encontrarCamino(boardInfo, currentPos, goals);
        }
        
        return pasos.Pop();
    }

    public override void Repath()
    {
        throw new System.NotImplementedException();
    }

    public void encontrarCamino(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        
        List<NodoEstrella> sucesores = new List<NodoEstrella>();
        
        nodoActual = new NodoEstrella();
        nodoActual.cell = currentPos;
       
        nodoActual.papi = null;
        abierta.Add(nodoActual);
        
        while (!caminoEncontrado)
        {

            if (abierta.Count == 0){caminoEncontrado = true;}
            else
            {
                nodoActual = abierta[0];
                abierta.RemoveAt(0);
                if (nodoActual.cell.CellId == goals[0].CellId)
                {
                    nodoMeta = nodoActual;
                    caminoEncontrado = true;
                }
                else
                {
                    sucesores = nodoActual.expandir(boardInfo, goals[0]);
                    foreach(NodoEstrella N in sucesores)
                    {
                        N.papi = nodoActual;
                        abierta.Add(N);
                        
                    }
                    abierta = abierta.OrderBy(n => n.fStar).ToList(); //ordena de menor a mayor f*
                }
            }
        }

		nodoActual = nodoMeta;

        while(nodoActual.papi != null)
        {
            pasos.Push(nodoActual.dir);
            nodoActual = nodoActual.papi;
            
        }
        
    }

	void Update () {
		
	}
}
