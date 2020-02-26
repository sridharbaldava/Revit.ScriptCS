using Autodesk.Revit.UI.Selection;

ISelectionFilter selFilter = new PlanarFacesSelectionFilter(doc);
try
{
    IList<Reference> faces = uidoc.Selection.PickObjects(ObjectType.Face, selFilter, "Select multiple planar faces");
    foreach (var face in faces)
    {
        Print(face.GlobalPoint.ToString());
    }
}
catch(Exception ex)
{
    Print(ex.Message);
}


private class PlanarFacesSelectionFilter : ISelectionFilter
{
    private readonly Document doc = null;
    public PlanarFacesSelectionFilter(Document document)
    {
        doc = document;
    }

    public bool AllowElement(Element element)
    {
        return true;
    }
    
    public bool AllowReference(Reference refer, XYZ point)
    {
        if (doc.GetElement(refer).GetGeometryObjectFromReference(refer) is PlanarFace)
        {
            return true; // Only return true for planar faces. Non-planar faces will not be selectable
        }
        return false;
    }
}
