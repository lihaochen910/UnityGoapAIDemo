namespace Nez.AI.GOAP {
	
	public class ActionImpl {
		
		public bool _finished;
		
		public void finish () {
			_finished = true;
		}
		
		public virtual void onEnter() 
		{}
		

		public virtual void onTick()
		{}

		
		public virtual void onExit()
		{}
	}
	
	public class ActionImpl<T> : ActionImpl {
		
		protected T _context;
		
		public ActionImpl ( T context ) {
			_context  = context;
		}
	}
}
