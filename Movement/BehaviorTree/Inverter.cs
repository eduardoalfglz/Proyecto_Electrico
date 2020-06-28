/**
 * Codigo obtenido de https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition 
 * Licencia MIT
 **/

using UnityEngine;
using System.Collections;


/**
 *@class Inverter
 *@brief Clase que hereda de nodo, cuenta con nodo hijo
 *@details El resutultado de este nodo es inverso al de su nodo hijo
 **/
public class Inverter : Node {
    /* Child node to evaluate */
    private Node m_node;

    public Node node {
        get { return m_node; }
    }

    /**
    *@funtion Inverter
    *@brief Constructor, requiere un nodo hijo para ser creado
    **/
    public Inverter(Node node) {
        m_node = node;
    }

    /**
    *@funtion Evaluate
    *@brief Evalua el resultado del nodo hijo
    *@details invierte el resultado del nodo hijo
    **/
    public override NodeStates Evaluate() {
        switch (m_node.Evaluate()) {
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
        }
        m_nodeState = NodeStates.SUCCESS;
        return m_nodeState;
    }
}
