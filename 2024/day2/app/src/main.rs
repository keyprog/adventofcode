use std::fs;
use std::fs::File;
use std::io;
use std::io::BufRead;

fn main() {
    let input_file = fs::canonicalize("../input.txt").unwrap();
    println!("Reading input from {:?}", input_file);

    let file = File::open(input_file).expect("Unable to open input file");

    let mut safe_reports_count = 0;
    for line in io::BufReader::new(file).lines().flatten() {
        if is_safe(line) {
            safe_reports_count = safe_reports_count + 1;
        }
    }

    println!("Safe reports: {safe_reports_count}");
}

fn is_safe(report: String) -> bool {
    println!("{report}");

    let levels = &report
        .split_whitespace()
        .map(|l| l.parse::<i32>().unwrap())
        .collect::<Vec<i32>>();

    sequence_is_safe(|(a, b)| a < b && b - a < 4, levels)
        || sequence_is_safe(|(a, b)| a > b && a - b < 4, levels)
}

fn sequence_is_safe(predicate: fn((&i32, &i32)) -> bool, collection: &Vec<i32>) -> bool {
    let pos = position_of_wrong_level(predicate, collection, None);
    match pos {
        None => true,
        Some(pos) => {
            position_of_wrong_level(predicate, collection, Some(pos)) == None
                || position_of_wrong_level(predicate, collection, Some(pos + 1)) == None
        }
    }
}

fn position_of_wrong_level(
    predicate: fn((&i32, &i32)) -> bool,
    collection: &Vec<i32>,
    ignore_pos: Option<usize>,
) -> Option<usize> {
    let mut curr = if ignore_pos != Some(0) { 0 } else { 1 };
    let mut next = if ignore_pos != Some(1) { curr + 1 } else { 2 };

    while next < collection.len() {
        if !predicate((&collection[curr], &collection[next])) {
            return Some(curr);
        }
        if let Some(position) = ignore_pos {
            curr = curr + 1;
            if curr == position {
                curr = curr + 1;
            }
            next = curr + 1;
            if (next == position) {
                next = next + 1;
            }
        } else {
            curr = curr + 1;
            next = curr + 1;
        }
    }
    None
}

/*fn sequence_is<'a>(
    predicate: fn((&i32, &i32)) -> bool,
    collection: impl Iterator<Item = &'a i32>,
    tolerance: i32,
    ignore_pos: usize,
) -> bool {
    if ignore_pos >= 0 {
        collection = collection
            .enumerate()
            .filter(|&(i, _)| i < 5)
            .map(|(_, e)| e);
    }

    let indexOfMismatch = collection
        .into_iter()
        .zip(collection.into_iter().skip(1))
        .position(|t| !predicate(t));
    if indexOfMismatch == None {
        return true;
    }

    match indexOfMismatch {
        None => true,
        Some(i) if tolerance > 0 => {
            sequence_is(predicate, &collection.clone().remove(i), tolerance - 1)
        }
    }
}*/
