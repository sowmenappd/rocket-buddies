using System.Collections.Generic;

public class TaskBuilder
{
    private static TaskBuilder instance;

    public static TaskBuilder I {
        get {
            if(instance == null) instance = new TaskBuilder();
            return instance;
        } private set {}
    }
    
    public Queue<System.Action> queue = new Queue<System.Action>();

    public TaskBuilder Chain(System.Action task){
        if(queue == null) queue = new Queue<System.Action>();
        queue.Enqueue(task);
        return this;
    }

    public void ExecuteTaskChain(){
        while(queue.Count > 0){
            var task = queue.Dequeue();
            task?.Invoke();
        }
    }

    
}
