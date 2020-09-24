    // Create a few geometry lines. These lines are transaction (not in the model),
    // therefore they do not need to be created inside a document transaction.
    XYZ Point1 = XYZ.Zero;
    XYZ Point2 = new XYZ(10, 0, 0);
    XYZ Point3 = new XYZ(10, 10, 0);
    XYZ Point4 = new XYZ(0, 10, 0);

    Line geomLine1 = Line.CreateBound(Point1, Point2);
    Line geomLine2 = Line.CreateBound(Point4, Point3);
    Line geomLine3 = Line.CreateBound(Point1, Point4);

    // This geometry plane is also transaction and does not need a transaction
    XYZ origin = XYZ.Zero;
    XYZ normal = new XYZ(0, 0, 1);
    Plane geomPlane = Plane.CreateByNormalAndOrigin(normal, origin);

    // In order to a sketch plane with model curves in it, we need
    // to start a transaction because such operations modify the model.

    // All and any transaction should be enclosed in a 'using'
    // block or guarded within a try-catch-finally blocks
    // to guarantee that a transaction does not out-live its scope.
    using (Transaction transaction = new Transaction(doc))
    {
        if (transaction.Start("Create model curves") == TransactionStatus.Started)
        {
            // Create a sketch plane in current document
            SketchPlane sketch = SketchPlane.Create(doc,geomPlane);

            // Create a ModelLine elements using the geometry lines and sketch plane
            ModelLine line1 = doc.Create.NewModelCurve(geomLine1, sketch) as ModelLine;
            ModelLine line2 = doc.Create.NewModelCurve(geomLine2, sketch) as ModelLine;
            ModelLine line3 = doc.Create.NewModelCurve(geomLine3, sketch) as ModelLine;

            // Ask the end user whether the changes are to be committed or not
            TaskDialog taskDialog = new TaskDialog("Revit");
            taskDialog.MainContent = "Click either [OK] to Commit, or [Cancel] to Roll back the transaction.";
            TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
            taskDialog.CommonButtons = buttons;

            if (TaskDialogResult.Ok == taskDialog.Show())
            {
                // For many various reasons, a transaction may not be committed
                // if the changes made during the transaction do not result a valid model.
                // If committing a transaction fails or is canceled by the end user,
                // the resulting status would be RolledBack instead of Committed.
                if (TransactionStatus.Committed != transaction.Commit())
                {
                    TaskDialog.Show("Failure", "Transaction could not be committed");
                }
            }
            else
            {
                transaction.RollBack();
            }
        }
    }
    
    return "Done";