using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Eb3yrLib.Kinematics
{
	// Use a chain like inverse kinematics, just treat it differently
	public static class Rope
	{
		public static void Custom(Chain chain, Vector2 gravity, float elasticity = 0f)	// Personally, I think an overload where gravity is a vector field rather than a constant would be really cool
		{
			// My immediate thought is displace chain elements based on gravity, then FABRIK it. Probably not a "correct" approach but it could be cool. Now we're getting into chain elements having mass and velocity and links having elasticity and all that really fun stuff. Chain elements are inevitably gonna end up being physics objects rather than Vector2s. Another advantage of physics objects is that their collision behaviour can be handled elsewhere in the code. Though, as collision isn't considered for each step in the algorithm, the timing may cause Shenanigans™

			// My other thought is that we run a force balance with gravity and a piecewise elastic force function for each joint connection (1-2 in this case, 1 on array bounds, 2 otherwise [I'd LOVE to make an N-connection solution some day] ). The elastic function is a function of distance between the joints, with force(dist < segment length) = 0, linear otherwise. I guess this gets me one big simultaneous equation for the entire linkage? Sounds slow as fuck, might need to reconsider or develop some iterative heuristic because I can't iamgine a fat N-by-N matrix being the best way to solve this.
			//		// Go pen-and-paper this bitch! And only touch the academic papers AFTER
		}
	}
}
