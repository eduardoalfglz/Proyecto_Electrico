/**
 * Codigo obtenido de https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition 
 * Licencia MIT
 **/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 *@class Sequence
 *@brief Clase que hereda de nodo, evalua nodos hijos de forma secuencial
 *@details Cuenta con varios nodos hijos y los evalua en orden secuencial, la condicion de retorno
 * depende de los nodos hijos
 **/
public class Sequence : Node {
    /** Chiildren nodes that belong to this sequence */
    private List<Node> m_nodes = new List<Node>();

    
    /**
    *@funtion Sequence
    *@brief Constructor, requiere nodos hijos para ser creado
    **/
    public Sequence(List<Node> nodes) {
        m_nodes = nodes;
    }

    /**
    *@funtion Evaluate
    *@brief Evalua la condicion de los nodos hijos segun prioridad
    *@details Inmediatamente retorna fallo en caso de que algun nodo falle, por eso es 
    * importante la prioriodad con la que se determinan los hijos, para poder retornar
    * exito todos los nodos tienen que tener exito
    **/
    public override NodeStates Evaluate() {
        //bool anyChildRunning = false; //Se cambia para que ahora sea una secuencia estricta
        //Debug.Log(m_nodes.Count);
        foreach(Node node in m_nodes) {
            //Debug.Log(node);
            switch (node.Evaluate()) {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;                    
                case NodeStates.SUCCESS:
                    continue;
                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;
                default:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
            }
        }
        //Debug.Log("Exito");
        m_nodeState = NodeStates.SUCCESS;
        //m_nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
        return m_nodeState;
    }
}
