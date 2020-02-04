LogicalOrFilter categoryFilter = new LogicalOrFilter(new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming), new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns));
LogicalAndFilter familySymbolFilter = new LogicalAndFilter(new ElementClassFilter(typeof(FamilySymbol)), categoryFilter);
var framingSymbolCollector = new FilteredElementCollector(doc).WherePasses(familySymbolFilter);
return framingSymbolCollector.OfType<FamilySymbol>().First().Name;