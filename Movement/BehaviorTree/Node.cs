/**
 * Codigo obtenido de https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition 
 * Licencia MIT
 **/


using UnityEngine;
using System.Collections;

[System.Serializable]
/**
 *@class Node
 *@brief Clase abstracta que define los nodos para el arbol de comportamiento
 *@details Un nodo puede ser de accion, seleccion o un decorador para mas detalles ver otras clases
 **/
public abstract class Node {

    /**
    *@brief Retorna el estado del nodo, basicamente un puntero
    **/
    public delegate NodeStates NodeReturn();
    //A delegate is a type that safely encapsulates a method, similar to a function pointer in C and C++. Unlike C function 
    //pointers, delegates are object-oriented, type safe, and secure

    /* The current state of the node */
    protected NodeStates m_nodeState;

    /**
    *@funtion nodeState
    *@brief Retorna el estado del nodo para que este no pueda ser modficado externamente
    **/
    public NodeStates nodeState {
        get { return m_nodeState; }
    }

    /**
    *@funtion Node
    *@brief Contructor Vacio
    **/
    public Node() {}

    /**
    *@funtion Evaluate
    *@brief Las clases implementadas utilizan este metodo para establecer las condiciones de retorno
    **/
    public abstract NodeStates Evaluate();

}
