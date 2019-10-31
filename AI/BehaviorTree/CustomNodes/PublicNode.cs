/**
*@class PublicNode
*@brief Clase que hereda de nodo, Nodo de estado modificable publicamente
*@details Es un nodo que me permite modificarlo publicamente
**/
public class PublicNode : Node
{


    /**
    *@funtion PublicNode
    *@brief Constructor vacio
    **/
    public PublicNode()
    {
        
    }

    /**
    *@funtion Evaluate
    *@brief Evalua la condicion de la accion
    *@details Inmediatamente retorna fallo el resultado de la accion
    **/
    public override NodeStates Evaluate()
    {
        
        return m_nodeState;
        
    }

    /**
    *@funtion ChangeState
    *@brief Cambia el estado del nodo actual
    **/
    public void ChangeState(NodeStates returnValue)
    {
        m_nodeState = returnValue;   
    }

}
