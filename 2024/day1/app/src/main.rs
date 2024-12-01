use std::cmp::Ordering;
use std::fs;
use std::fs::File;
use std::io::{self, BufRead};

fn main() {
    let input_file = fs::canonicalize("../input.txt").unwrap();
    println!("Reading input from file: {:?}", input_file);

    let file = File::open(input_file).unwrap();
    let mut left: Vec<i32> = Vec::new();
    let mut right: Vec<i32> = Vec::new();

    for line in io::BufReader::new(file).lines().flatten() {
        if let Some((a, b)) = line.split_once(' ') {
            left.push(a.trim().parse().unwrap());
            right.push(b.trim().parse().unwrap());
        }
    }
    left.sort();
    right.sort();

    let mut distance = 0;
    for i in 0..left.len() {
        distance = distance + (right[i] - left[i]).abs();
    }
    println!("Total distance: {distance}");

    let mut left_cur = 0;
    let mut right_cur = 0;
    let mut similarity_score: i64 = 0;
    while left_cur < left.len() && right_cur < right.len() {
        match left[left_cur].cmp(&right[right_cur]) {
            Ordering::Equal => {
                similarity_score = similarity_score + i64::from(left[left_cur]);
                right_cur = right_cur + 1
            }
            Ordering::Less => left_cur = left_cur + 1,
            Ordering::Greater => right_cur = right_cur + 1,
        }
        if right_cur >= right.len() {
            break;
        }
    }
    println!("Similarity score: {similarity_score}");
}
