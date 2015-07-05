using UnityEngine;
using System.Collections.Generic;
using UIWidgets;

public class TestListViewIcons : MonoBehaviour {
	[SerializeField]
	ListViewIcons listViewIcons;
	
	[SerializeField]
	Sprite sampleIcon;


	void Start (){
	
		Test ();
	}


	public void Test()
	{
		//Get last selected index
		Debug.Log(listViewIcons.SelectedIndex);
		
		//Get selected indicies
		var indicies = listViewIcons.SelectedIndicies;
		Debug.Log(string.Join(", ", indicies.ConvertAll(x => x.ToString()).ToArray()));
		
		//Get last selected string
		Debug.Log(listViewIcons.SelectedItem.Name);
		
		//Get selected strings
		var selected_items = listViewIcons.SelectedItems;
		Debug.Log(string.Join(", ", selected_items.ConvertAll(x => x.Name).ToArray()));
		
		//Deleting specified item
		var items = listViewIcons.Items;
		listViewIcons.Remove(items[0]);
		
		//Deleting item by index
		listViewIcons.Remove(0);
		
		//Clear List
		listViewIcons.Clear();
		
		//Add item
		var new_item = new ListViewIconsItemDescription() {
			Icon = sampleIcon,
			Name = "test item"
		};
		listViewIcons.Add(new_item);
		
		//Add items
		var new_items = new List<ListViewIconsItemDescription>() {
			new_item,
			new_item,
			new_item
		};
		new_items.AddRange(listViewIcons.Items);
		listViewIcons.Items = new_items;
		
		//Set selected index
		listViewIcons.SelectedIndex = 1;
		//or
		listViewIcons.Select(1);
	}
}