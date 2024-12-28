use std::{fmt, fs, io::Read};

fn main() {
    let mut data: Vec<Vec<char>> = fs::read_to_string("./input.txt")
        .unwrap()
        .lines()
        .map(|d| d.chars().collect())
        .collect();
    let word = &"XMAS".to_string();
    let mut words_found = 0;

    let size = data.len();
    for _rotation in 0..4 {
        for row in 0..size {
            for col in 0..size {
                words_found = words_found + search_word(&data, row, col, word)
            }
        }
        rotate(&mut data);
    }

    println!("Part I: Total words found: {words_found}");

    let m_xmas = vec![
        vec!['M', '.', 'S'],
        vec!['.', 'A', '.'],
        vec!['M', '.', 'S'],
    ];
    words_found = 0;
    for _rotation in 0..4 {
        for row in 0..size {
            for col in 0..size {
                words_found = words_found + search_matrix(&data, row, col, &m_xmas)
            }
        }
        rotate(&mut data);
    }
    println!("Part II: Total number of X-MAS: {words_found}");
}

fn search_word(data: &Vec<Vec<char>>, row: usize, col: usize, word: &str) -> i32 {
    search_word_right(data, row, col, word) + search_word_diag(data, row, col, word)
}

fn search_word_right(data: &Vec<Vec<char>>, row: usize, col: usize, word: &str) -> i32 {
    if data[row].len() - col < word.len() {
        return 0;
    }
    let mut word_chars = word.chars();
    for i in 0usize..word.len() {
        if data[row][col + i] != word_chars.next().unwrap() {
            return 0;
        }
    }
    return 1;
}

fn search_word_diag(data: &Vec<Vec<char>>, row: usize, col: usize, word: &str) -> i32 {
    let mut word_chars = word.chars();
    if data[row].len() - col < word.len() || data[row].len() - row < word.len() {
        return 0;
    }
    for i in 0usize..word.len() {
        if data[row + i][col + i] != word_chars.next().unwrap() {
            return 0;
        }
    }
    return 1;
}
fn search_matrix(data: &Vec<Vec<char>>, row: usize, col: usize, matrix: &Vec<Vec<char>>) -> i32 {
    let size = matrix.len();
    if data.len() - row < size || data[row].len() - col < size {
        return 0;
    }
    for r in 0..size {
        for c in 0..size {
            //println!("{:?} {:?}", data[row + r][col + c], matrix[r][c]);
            if data[row + r][col + c] != matrix[r][c] && matrix[r][c] != '.' {
                return 0;
            }
        }
    }
    1
}
fn rotate(data: &mut Vec<Vec<char>>) {
    rotate_at(data, 0);
}
fn rotate_at(data: &mut Vec<Vec<char>>, offset: usize) {
    let height = data.len();
    if height / 2 < offset {
        return;
    }

    let x = offset;
    for y in offset..height - offset - 1 {
        let tmp = data[x][y];
        data[x][y] = data[y][height - x - 1];
        data[y][height - x - 1] = data[height - x - 1][height - y - 1];
        data[height - x - 1][height - y - 1] = data[height - y - 1][x];
        data[height - y - 1][x] = tmp;
    }
    rotate_at(data, offset + 1);
}

pub fn print_vec(data: &Vec<Vec<char>>) {
    for l in data {
        println!("{:?}", l);
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn rotate_test() {
        let mut orig = vec![
            vec!['1', '2', '3'],
            vec!['4', '5', '6'],
            vec!['7', '8', '9'],
        ];
        let expected = vec![
            vec!['3', '6', '9'],
            vec!['2', '5', '8'],
            vec!['1', '4', '7'],
        ];
        print_vec(&orig);
        rotate(&mut orig);
        print_vec(&orig);
        assert_eq!(orig, expected);
    }
}
