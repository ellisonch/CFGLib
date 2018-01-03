using CFGLib.ProductionAnnotations.Actioning;
using CFGLib.ProductionAnnotations.Gathering;
using CFGLib.ProductionAnnotations.Precedencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.ProductionAnnotations {
	public class Annotations {
		private static Annotations _emptyAnnotations = new Annotations(Enumerable.Empty<IAnnotation>());
		// private Dictionary<string, object> _dictionary = new Dictionary<string, object>();
		public ActionAnnotation Action { get; }
		public GatherAnnotation Gather { get; }
		public PrecedenceAnnotation Precedence { get; }
		public IEnumerable<IAnnotation> AllAnnotations { get; }
		public static Annotations Empty {
			get {
				return _emptyAnnotations;
			}
		}

		public Annotations(IEnumerable<IAnnotation> annotations) {
			foreach (var annotation in annotations) {
				if (annotation is ActionAnnotation action) {
					Action = action;
				} else if (annotation is GatherAnnotation gather) {
					Gather = gather;
				} else if (annotation is PrecedenceAnnotation precedence) {
					Precedence = precedence;
				}
			}
			AllAnnotations = annotations;
		}
	}
}
