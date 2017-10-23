module DarkTestCode exposing (..)

import Types exposing (..)

functions : List Function
functions = [ { name = "%", description = "", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "&&", description = "Returns true if both a and b are true", returnTipe = TBool, parameters = [ { name = "a", tipe = TBool, block_args = [], optional = False, description = "" }, { name = "b", tipe = TBool, block_args = [], optional = False, description = "" } ] }, { name = "*", description = "Multiples two integers", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "+", description = "Adds two integers together", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "-", description = "Subtracts two integers", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = ".", description = "", returnTipe = TAny, parameters = [ { name = "value", tipe = TObj, block_args = [], optional = False, description = "" }, { name = "fieldname", tipe = TStr, block_args = [], optional = False, description = "" } ] }, { name = "/", description = "Divides two integers", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "<", description = "Returns true if a is less than b", returnTipe = TBool, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "<=", description = "Returns true if a is less than or equal to b", returnTipe = TBool, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "==", description = "Returns true if the two value are equal", returnTipe = TBool, parameters = [ { name = "a", tipe = TAny, block_args = [], optional = False, description = "" }, { name = "b", tipe = TAny, block_args = [], optional = False, description = "" } ] }, { name = ">", description = "Returns true if a is greater than b", returnTipe = TBool, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = ">=", description = "Returns true if a is greater than or equal to b", returnTipe = TBool, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Bool::and", description = "Returns true if both a and b are true", returnTipe = TBool, parameters = [ { name = "a", tipe = TBool, block_args = [], optional = False, description = "" }, { name = "b", tipe = TBool, block_args = [], optional = False, description = "" } ] }, { name = "Bool::not", description = "", returnTipe = TBool, parameters = [ { name = "b", tipe = TBool, block_args = [], optional = False, description = "" } ] }, { name = "Bool::or", description = "Returns true if either a is true or b is true", returnTipe = TBool, parameters = [ { name = "a", tipe = TBool, block_args = [], optional = False, description = "" }, { name = "b", tipe = TBool, block_args = [], optional = False, description = "" } ] }, { name = "Char::toASCIIChar", description = "", returnTipe = TChar, parameters = [ { name = "i", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Char::toASCIICode", description = "Return `c`'s ASCII code", returnTipe = TInt, parameters = [ { name = "c", tipe = TChar, block_args = [], optional = False, description = "" } ] }, { name = "Char::toLowercase", description = "Return the lowercase value of `c`", returnTipe = TChar, parameters = [ { name = "c", tipe = TChar, block_args = [], optional = False, description = "" } ] }, { name = "Char::toUppercase", description = "Return the uppercase value of `c`", returnTipe = TChar, parameters = [ { name = "c", tipe = TChar, block_args = [], optional = False, description = "" } ] }, { name = "Date::now", description = "Returns the number of seconds since the epoch (midnight, Jan 1, 1970)", returnTipe = TInt, parameters = [] }, { name = "Date::parse", description = "Parses a time string, and return the number of seconds since the epoch (midnight, Jan 1, 1970)", returnTipe = TInt, parameters = [ { name = "s", tipe = TStr, block_args = [], optional = False, description = "" } ] }, { name = "Dict::keys", description = "", returnTipe = TList, parameters = [ { name = "dict", tipe = TObj, block_args = [], optional = False, description = "" } ] }, { name = "Int::add", description = "Adds two integers together", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Int::divide", description = "Divides two integers", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Int::greaterThan", description = "Returns true if a is greater than b", returnTipe = TBool, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Int::greaterThanOrEqualTo", description = "Returns true if a is greater than or equal to b", returnTipe = TBool, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Int::lessThan", description = "Returns true if a is less than b", returnTipe = TBool, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Int::lessThanOrEqualTo", description = "Returns true if a is less than or equal to b", returnTipe = TBool, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Int::mod", description = "", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Int::multiply", description = "Multiples two integers", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "Int::subtract", description = "Subtracts two integers", returnTipe = TInt, parameters = [ { name = "a", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "b", tipe = TInt, block_args = [], optional = False, description = "" } ] }, { name = "List::append", description = "Returns the combined list of `l1` and `l2`", returnTipe = TList, parameters = [ { name = "l1", tipe = TList, block_args = [], optional = False, description = "" }, { name = "l2", tipe = TList, block_args = [], optional = False, description = "" } ] }, { name = "List::contains", description = "Returns if the value is in the list", returnTipe = TBool, parameters = [ { name = "l", tipe = TList, block_args = [], optional = False, description = "" }, { name = "val", tipe = TAny, block_args = [], optional = False, description = "" } ] }, { name = "List::empty", description = "", returnTipe = TList, parameters = [] }, { name = "List::filter", description = "Return only values in `l` which meet the function's criteria", returnTipe = TList, parameters = [ { name = "l", tipe = TList, block_args = [], optional = False, description = "" }, { name = "f", tipe = TBlock, block_args = [ "val" ], optional = False, description = "" } ] }, { name = "List::find_first", description = "Find the first element of the list, for which `f` returns true", returnTipe = TList, parameters = [ { name = "l", tipe = TList, block_args = [], optional = False, description = "" }, { name = "f", tipe = TBlock, block_args = [ "val" ], optional = False, description = "" } ] }, { name = "List::flatten", description = "Returns a single list containing the elements of all the lists in `l`", returnTipe = TList, parameters = [ { name = "l", tipe = TList, block_args = [], optional = False, description = "" } ] }, { name = "List::fold", description = "Folds the list into a single value, by repeatedly apply `f` to any two pairs", returnTipe = TAny, parameters = [ { name = "l", tipe = TList, block_args = [], optional = False, description = "" }, { name = "init", tipe = TAny, block_args = [], optional = False, description = "" }, { name = "f", tipe = TBlock, block_args = [ "accum", "curr" ], optional = False, description = "" } ] }, { name = "List::foreach", description = "Call `f` on every item in the list, returning a list of the results of\n  those calls", returnTipe = TList, parameters = [ { name = "l", tipe = TList, block_args = [], optional = False, description = "" }, { name = "f", tipe = TBlock, block_args = [ "val" ], optional = False, description = "" } ] }, { name = "List::head", description = "", returnTipe = TAny, parameters = [ { name = "list", tipe = TList, block_args = [], optional = False, description = "" } ] }, { name = "List::last", description = "", returnTipe = TAny, parameters = [ { name = "list", tipe = TList, block_args = [], optional = False, description = "" } ] }, { name = "List::length", description = "Returns the length of the list", returnTipe = TInt, parameters = [ { name = "l", tipe = TList, block_args = [], optional = False, description = "" } ] }, { name = "List::new", description = "Return a new list with the arguments provided", returnTipe = TList, parameters = [ { name = "i1", tipe = TAny, block_args = [], optional = True, description = "" }, { name = "i2", tipe = TAny, block_args = [], optional = True, description = "" }, { name = "i3", tipe = TAny, block_args = [], optional = True, description = "" }, { name = "i4", tipe = TAny, block_args = [], optional = True, description = "" }, { name = "i5", tipe = TAny, block_args = [], optional = True, description = "" }, { name = "i6", tipe = TAny, block_args = [], optional = True, description = "" } ] }, { name = "List::push", description = "", returnTipe = TList, parameters = [ { name = "val", tipe = TAny, block_args = [], optional = False, description = "" }, { name = "list", tipe = TList, block_args = [], optional = False, description = "" } ] }, { name = "List::range", description = "Return a list of increasing integers from `start` to `stop`, inclusive", returnTipe = TList, parameters = [ { name = "start", tipe = TInt, block_args = [], optional = False, description = "First number in the range, will be included" }, { name = "stop", tipe = TInt, block_args = [], optional = False, description = "Last number in the range, is included" } ] }, { name = "List::repeat", description = "Returns a list containing `val` repeated `count` times", returnTipe = TList, parameters = [ { name = "times", tipe = TInt, block_args = [], optional = False, description = "" }, { name = "val", tipe = TAny, block_args = [], optional = False, description = "" } ] }, { name = "Page::GET", description = "Create a page at `url` that prints `value`", returnTipe = TNull, parameters = [ { name = "url", tipe = TStr, block_args = [], optional = False, description = "" }, { name = "val", tipe = TAny, block_args = [], optional = False, description = "" } ] }, { name = "String::foreach", description = "Iterate over each character in the string, performing the operation in the block on each one", returnTipe = TStr, parameters = [ { name = "s", tipe = TStr, block_args = [], optional = False, description = "" }, { name = "f", tipe = TBlock, block_args = [ "char" ], optional = False, description = "" } ] }, { name = "String::fromList", description = "Returns the list of characters as a string", returnTipe = TStr, parameters = [ { name = "l", tipe = TList, block_args = [], optional = False, description = "" } ] }, { name = "String::toList", description = "Returns the list of characters in the string", returnTipe = TList, parameters = [ { name = "s", tipe = TStr, block_args = [], optional = False, description = "" } ] }, { name = "_", description = "Ignores the first param and returns the 2nd.", returnTipe = TAny, parameters = [ { name = "ignore", tipe = TAny, block_args = [], optional = False, description = "" }, { name = "value", tipe = TAny, block_args = [], optional = False, description = "" } ] }, { name = "equals", description = "Returns true if the two value are equal", returnTipe = TBool, parameters = [ { name = "a", tipe = TAny, block_args = [], optional = False, description = "" }, { name = "b", tipe = TAny, block_args = [], optional = False, description = "" } ] }, { name = "get_field", description = "", returnTipe = TAny, parameters = [ { name = "value", tipe = TObj, block_args = [], optional = False, description = "" }, { name = "fieldname", tipe = TStr, block_args = [], optional = False, description = "" } ] }, { name = "if", description = "If cond is true, calls the `then` function. Otherwise calls the `else`\n  function. Both functions get 'v' piped into them", returnTipe = TAny, parameters = [ { name = "v", tipe = TAny, block_args = [], optional = False, description = "" }, { name = "cond", tipe = TBool, block_args = [], optional = False, description = "" }, { name = "ftrue", tipe = TBlock, block_args = [ "then" ], optional = False, description = "" }, { name = "ffalse", tipe = TBlock, block_args = [ "else" ], optional = False, description = "" } ] }, { name = "toString", description = "Returns a string representation of `v`", returnTipe = TStr, parameters = [ { name = "v", tipe = TAny, block_args = [], optional = False, description = "" } ] }, { name = "||", description = "Returns true if either a is true or b is true", returnTipe = TBool, parameters = [ { name = "a", tipe = TBool, block_args = [], optional = False, description = "" }, { name = "b", tipe = TBool, block_args = [], optional = False, description = "" } ] } ]
