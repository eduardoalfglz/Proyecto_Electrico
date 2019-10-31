/**
 * Codigo obtenido de https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition 
 * Licencia MIT
 **/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 *@class Selector
 *@brief Clase que hereda de nodo, selecciona entre nodos hijos
 *@details Cuenta con varios nodos hijos y los evalua en orden de prioridad, la condicion de retorno
 * depende de los nodos hijos
 **/
public class Selector : Node
{
    /** Nodos hijos de selector*/
    private List<Node> m_nodes = new List<Node>();



    /**
    *@funtion Selector
    *@brief Constructor, requiere nodos hijos para ser creado
    **/
    public Selector(List<Node> nodes)
    {
        m_nodes = nodes;
    }

    /**
    *@funtion Evaluate
    *@brief Evalua la condicion de los nodos hijos segun prioridad
    *@details Inmediatamente retorna exito en caso de que algun nodo tenga exito, por eso es 
    * importante la prioriodad con la que se determinan los hijos
    **/
    public override NodeStates Evaluate()
    {
        //Debug.Log(m_nodes.Count);
        foreach (Node node in m_nodes){
            switch (node.Evaluate())
            {
                case NodeStates.FAILURE:
                    continue;
                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
                default:
                    continue;
            }
        }
        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}
