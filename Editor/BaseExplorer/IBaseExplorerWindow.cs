using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MM.BaseExplorer
{
	public interface IBaseExplorerWindow
	{
		bool IsSelected(BaseExplorerItem i_Item);
		void AddSelected(params BaseExplorerItem[] i_Items);
		void SetSelected(params BaseExplorerItem[] i_Items);
		void RemoveSelected(params BaseExplorerItem[] i_Items);

		float GetListWidth();
		float GetInspectorWidth();
	}
}
