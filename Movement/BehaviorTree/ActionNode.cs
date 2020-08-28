/**
 * Codigo obtenido de https://github.com/PacktPublishing/Unity-2017-Game-AI-Programming-Third-Edition 
 * Licencia MIT
 **/

/**
*@class ActionNode
*@brief Clase que hereda de nodo, Nodo que ejecuta la accion
*@details Es el nodo mas importante del arbol ya que es el que realiza la accion, es la "hoja del arbol"
**/
public class ActionNode : Node {
    /* Funcion asociada al nodo */
    public delegate NodeStates ActionNodeDelegate();

    /* The delegate that is called to evaluate this node */
    private ActionNodeDelegate m_action;

    /**
    *@funtion ActionNode
    *@brief Constructor, requiere de una accion para ser creado
    *@details La accion asociada debe retornar NodeStates para que sea congruente
    **/
    public ActionNode(ActionNodeDelegate action) {
        m_action = action;
    }

    /**
    *@funtion Evaluate
    *@brief Evalua la condicion de la accion
    *@details Inmediatamente retorna fallo el resultado de la accion
    **/
    public override NodeStates Evaluate() {
        switch (m_action()) {
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
            default:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
        }
    }

}
