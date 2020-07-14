//
// CmdDuplicateElement.cs - duplicate selected elements
//
// Copyright (C) 2010-2020 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//
using (Transaction tx = new Transaction(doc))
{
    tx.Start("Duplicate Elements");

    //Group group = doc.Create.NewGroup( // 2012
    //  uidoc.Selection.Elements );

    Group group = doc.Create.NewGroup( // 2013
      uidoc.Selection.GetElementIds());

    LocationPoint location = group.Location
      as LocationPoint;

    XYZ p = location.Point;
    XYZ newPoint = new XYZ(p.X, p.Y + 10, p.Z);

    Group newGroup = doc.Create.PlaceGroup(
      newPoint, group.GroupType);

    //group.Ungroup(); // 2012
    group.UngroupMembers(); // 2013

    //ElementSet eSet = newGroup.Ungroup(); // 2012

    ICollection<ElementId> eIds
      = newGroup.UngroupMembers(); // 2013

    // change the property or parameter values
    // of the member elements as required...

    tx.Commit();
    Print("Success");
}
