using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib {
	public interface IDirtyable {
		void SetDirty();
	}
	// this is a trick to get around the lack of Class<T> type inference in C#
	internal static class Cache {
		public static Cache<T> Create<T>(Func<T> build) {
			return Cache<T>.Create(build);
		}
	}
	internal class Cache<TValue> : IDirtyable {
		private bool _dirty = true;
		private TValue _value;
		private readonly Func<TValue> _build;

		private Cache(Func<TValue> buildCommand) {
			_build = buildCommand;
		}
		public static Cache<T> Create<T>(Func<T> build) {
			return new Cache<T>(build);
		}
		
		public TValue Value {
			get {
				if (_dirty) {
					Build();
				}
				return _value;
			}
		}

		private void Build() {
			_value = _build();
			_dirty = false;
		}

		public void SetDirty() {
			_dirty = true;
		}
	}
}
