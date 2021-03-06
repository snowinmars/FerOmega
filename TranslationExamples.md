# FerOmega output examples

## Advices

- Do escape with `[]` every string. Because `[boring]` will be translated as `'boring'`, but `boring` - as `'b' or 'ing'`.
- Beware of operators' priority. Check out the `/Services/configs/` config prioritize. By default, the sql provider will restore all ambivalent brackets, f.e. `1 + 2 + 3` will be transformed into `( 1 + 2 ) + 3` to expose that it's ok.
## Examples

#### Simple math

It removes unnecessary brackets.

- Input query: `([age] >= 16 and [country] == [ru]) or ([age] >= 13 and [country] === [ja])`
- Allowed properties:
    - `'age' -> 'age'`
    - `'country' -> 'country'` 
- Output query: `where age >= @3 and country = @2 or age >= @1 and country = @0`
- Output properties: `["ja", 13, "ru", 16]`

#### Range math

Each sql operator will be wrapped with spaces.

- Input query: `[location] in ([Moscow], [St. Petersburg]) and [country] in ([ru], [us])`
- Allowed properties:
    - `'location' -> 'location'`
    - `'country' -> 'country'`
- Output query: `where location in ( @3 , @2 ) and country in ( @1 , @0 )`
- Output properties: `["us", "ru", "St. Petersburg", "Moscow"]`

#### Property remapping

It will allow filtering for complex queries.

- Input query: `[storesCount] > 3 or [salaryCostsPerMonth] > 10000 * [dollarCourse]`
- Allowed properties:
    - `'storesCount' -> 'storesCount'`
    - `'salaryCostsPerMonth' -> salarySlice.perMonth` 
    - `'dollarCourse' -> 'bank.currentDollarCourse'`
- Output query: `where storesCount > @1 or salarySlice.perMonth > @0 * bank.currentDollarCourse`
- Output properties: `[10000, 3]`

#### String like

Having more/less then enough spaces is fine

- Input query: `[name] contains [and] or ([name]   startsWith   [Alex] and [name]endsWith[ndr])`
- Allowed properties:
    - `'name' -> 'name'`
- Output query: `where name like @2 or name like @1 and name like @0`
- Output properties: `["%ndr", "Alex%", "%and%"]`
