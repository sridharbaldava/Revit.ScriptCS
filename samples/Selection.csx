try
{
    // Get the element selection of current document.
    var selection = uidoc.Selection;
    ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

    if (0 == selectedIds.Count)
    {
        // If no elements selected.
        TaskDialog.Show("Revit", "You haven't selected any elements.");
    }
    else
    {
        String info = "Ids of selected elements in the document are: ";
        foreach (ElementId id in selectedIds)
        {
            info += "\n\t" + id.IntegerValue;
        }

        TaskDialog.Show("Revit", info);
    }
}
catch (Exception e)
{
    return e;
}

return "Done";
