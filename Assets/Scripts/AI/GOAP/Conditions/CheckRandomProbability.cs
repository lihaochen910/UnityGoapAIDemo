
namespace Nez.AI.GOAP {
	
	public class CheckRandomProbability : AICondition {
		
		public float probability;
		
		public override bool OnCheck () {
			return MathUtils.RandomProbability ( probability );
		}
	}
}
