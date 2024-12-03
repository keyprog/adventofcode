use std::{fmt::format, fs};

const TOKEN_MUL: &str = "mul";
const TOKEN_OPEN_BR: &str = "(";
const TOKEN_CLOSE_BR: &str = ")";
const TOKEN_COMMA: &str = ",";
const TOKEN_DO: &str = "do()";
const TOKEN_DONT: &str = "don't()";

fn main() {
    let input_file = fs::canonicalize("../input.txt").unwrap();
    println!("Reading input from {:?}", input_file);

    let content = fs::read_to_string(&input_file).unwrap();
    println!("Content:\n{}", content);

    let mut expected_token = TOKEN_MUL;
    let mut mul1 = 0;
    let mut mul2 = 0;
    let mut product: u64 = 0;
    let mut mul_start = 0;

    let mut i = 0usize;
    while i < content.len() - expected_token.len() {
        let token = &content[i..i + expected_token.len()];

        if token == expected_token {
            println!("Found {token} at {i}");
            i = i + token.len();
            match token {
                TOKEN_MUL => {
                    expected_token = TOKEN_OPEN_BR;
                    mul_start = i - token.len();
                }
                TOKEN_OPEN_BR => {
                    if let (Some(num), len) = parse_num(&content[i..]) {
                        mul1 = num;
                        i = i + len;
                        expected_token = TOKEN_COMMA;
                    } else {
                        expected_token = TOKEN_MUL;
                    }
                }
                TOKEN_COMMA => {
                    if let (Some(num), len) = parse_num(&content[i..]) {
                        mul2 = num;
                        i = i + len;
                        expected_token = TOKEN_CLOSE_BR;
                    } else {
                        expected_token = TOKEN_MUL;
                    }
                }
                TOKEN_CLOSE_BR => {
                    let result = mul1 * mul2;
                    let str: &str = &content[mul_start..i];
                    let actual_str = format!("mul({mul1},{mul2})");
                    product = product + u64::from(result);

                    println!("{actual_str} = {result} from '{str}'");
                    expected_token = TOKEN_MUL;
                }
                TOKEN_DO => {
                    expected_token = TOKEN_MUL;
                }
                _ => panic!("Not implemented"),
            }
        } else if content[i..].starts_with(TOKEN_DONT) {
            expected_token = TOKEN_DO;
            i = i + TOKEN_DONT.len();
        } else {
            if expected_token == TOKEN_MUL || expected_token == TOKEN_DO {
                i = i + 1;
            } else {
                expected_token = TOKEN_MUL;
            }
        }
    }
    println!("Result: {product}");
}

const MAX_NUM_LEN: usize = 3;
const ZERO: u8 = '0' as u8;
const TEN: u32 = 10;

fn parse_num(content: &str) -> (Option<u32>, usize) {
    let mut num: u32 = 0;
    let mut i: usize = 0;
    let max_len = if content.len() > MAX_NUM_LEN {
        MAX_NUM_LEN
    } else {
        content.len()
    };
    let bytes = content[0..max_len].as_bytes();
    if max_len == 0 || !bytes[0].is_ascii_digit() {
        return (None, 0usize);
    }

    while i < max_len {
        if bytes[i].is_ascii_digit() {
            num = num * 10 + u32::from(bytes[i] - ZERO);
        } else {
            break;
        }
        i = i + 1;
    }
    println!("Found {num}");
    return (Some(num), i);
}
