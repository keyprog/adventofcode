use std::cmp::Ordering;
use std::collections::HashSet;
use std::fs::File;
use std::io::{self, prelude::*, BufReader};

fn main() -> io::Result<()> {
    let input_file = File::open("input.txt")?;
    let input_reader = BufReader::new(input_file);

    let mut ordering_rules: HashSet<(i32, i32)> = HashSet::new();
    let mut reading_rules = true;

    let mut correctly_ordered_result: i32 = 0;
    let mut incorrectly_ordered_result: i32 = 0;

    for line in input_reader.lines() {
        if line.is_ok() {
            let line = line.unwrap();
            if line == "" {
                reading_rules = false;
                continue;
            }
            if reading_rules {
                if let Some((a, b)) = line.split_once('|') {
                    ordering_rules.insert((a.parse().unwrap(), b.parse().unwrap()));
                }
            } else {
                let mut message: Vec<i32> =
                    line.split(",").map(|s| s.parse::<i32>().unwrap()).collect();
                if !is_already_sorted(&message, &ordering_rules) {
                    sort(&ordering_rules, &mut message);

                    let mid_value = message[message.len() / 2];
                    incorrectly_ordered_result += mid_value;
                } else {
                    let mid_value = message[message.len() / 2];
                    correctly_ordered_result += mid_value;
                }
            }
        }
    }
    println!("Correctly Ordered Result: {}", correctly_ordered_result);
    println!("Incorrectly Ordered Result: {}", incorrectly_ordered_result);
    Ok(())
}

fn sort(ordering_rules: &HashSet<(i32, i32)>, message: &mut Vec<i32>) {
    message.sort_by(|a, b| {
        if a == b {
            return Ordering::Equal;
        } else if ordering_rules.contains(&(*a, *b)) {
            return Ordering::Less;
        } else {
            return Ordering::Greater;
        }
    });
}


fn is_already_sorted(input_values: &Vec<i32>, rules: &HashSet<(i32,i32)>) -> bool {
    input_values.iter().zip(input_values.iter().skip(1))
        .all(|(a, b)| rules.contains(&(*a, *b)))
}
