//
// CmdChangeElementColor.cs - Change element colour using OverrideGraphicSettings for active view
//
// Also change its category's material to a random material
//
// Copyright (C) 2020 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//
using Autodesk.Revit.UI.Selection;

View view = doc.ActiveView;
ElementId id;

try
{
    Selection sel = uidoc.Selection;
    Reference r = sel.PickObject(ObjectType.Element, "Pick element to change its colour");
    id = r.ElementId;
}
catch (Autodesk.Revit.Exceptions.OperationCanceledException)
{
    return Result.Cancelled;
}

ChangeElementColor(doc, id);

ChangeElementMaterial(doc, id);

return Result.Succeeded;

void ChangeElementColor(Document doc, ElementId id)
{
    Color color = new Color(
      (byte)200, (byte)100, (byte)100);

    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
    ogs.SetProjectionLineColor(color);

    using (Transaction tx = new Transaction(doc))
    {
        tx.Start("Change Element Color");
        doc.ActiveView.SetElementOverrides(id, ogs);
        tx.Commit();
    }
}

void ChangeElementMaterial(Document doc, ElementId id)
{
    Element e = doc.GetElement(id);

    if (null != e.Category)
    {
        int im = e.Category.Material.Id.IntegerValue;

        List<Material> materials = new List<Material>(
          new FilteredElementCollector(doc)
            .WhereElementIsNotElementType()
            .OfClass(typeof(Material))
            .ToElements()
            .Where<Element>(m
             => m.Id.IntegerValue != im)
            .Cast<Material>());

        Random r = new Random();
        int i = r.Next(materials.Count);

        using (Transaction tx = new Transaction(doc))
        {
            tx.Start("Change Element Material");
            e.Category.Material = materials[i];
            tx.Commit();
        }
    }
}